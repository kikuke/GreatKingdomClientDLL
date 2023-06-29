using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreatKingdomClient
{

    public class NetworkClient
    {
        const int BUFFER_MAX_SIZE = 2048*2*2*2;

        private TcpClient tc;
        private NetworkStream stream;
        private Queue<GameRoomInfo> updateQueue = new Queue<GameRoomInfo>();

        bool isReadThread;
        private Thread readPacketThread;
        private object readLock = new object();

        public NetworkClient(string serverIP, int serverPort)
        {
            tc = new TcpClient(serverIP, serverPort);
            stream = tc.GetStream();
        }

        ~NetworkClient()
        {
            tc.Close();
        }

        //업데이트 정보가 있는가
        public bool IsUpdate()
        {
            bool isUpdate;

            lock (readLock)
            {
                isUpdate = updateQueue.Count > 0;
            }
            return isUpdate;
        }

        //업데이트 정보 얻기
        public GameRoomInfo GetUpdateData()
        {
            GameRoomInfo info;

            lock (readLock)
            {
                info = updateQueue.Dequeue();
            }
            return info;
        }

        public int SetGameID(int id)
        {
            return SendSetClntIDPacket(id);
        }

        public RoomDatas GetGameRooms(int offset)
        {
            return SendGetGameRoomPacket(offset);
        }

        public int CreateGameRoom(int roomID)
        {
            return SendCreateGameRoomPacket(roomID);
        }

        public int JoinGameRoom(int roomID)
        {
            if (SendJoinGameRoomPacket(roomID) < 0)
                return -1;

            StartReadThread();
            return 0;
        }

        public void UpdateGameRoom(GameRoomInfo roomInfo)
        {
            SendUpdateGameRoomPacket(roomInfo);
        }

        public int OutGameRoom(int roomID)
        {
            EndReadThread();
            if (SendOutGameRoomPacket(roomID) < 0)
                return -1;

            return 0;
        }

        private void StartReadThread()
        {
            readPacketThread = new Thread(new ThreadStart(ReadPacket));
            isReadThread = true;
            readPacketThread.Start();
        }

        private void EndReadThread()
        {
            isReadThread = false;
            readPacketThread.Join();
        }

        //Todo: 룸 업데이트 정보 패킷일 경우 큐에 집어넣는 기능
        private void ReadPacket()
        {
            while (isReadThread)
            {
                byte[] buffer = new byte[BUFFER_MAX_SIZE];
                ReturnRoomData retData;

                if (ReadData(buffer, out retData) < 0)
                {
                    Console.WriteLine("read data failed!");
                    stream.Flush();
                    return;
                }

                lock(readLock)
                {
                    updateQueue.Enqueue(retData.roomInfo);
                }
                Console.WriteLine("새 데이터가 들어왔습니다.");
            }
        }

        //Todo: 룸 정보 업데이트 보내는 패킷
        private int SendSetClntIDPacket(int clnt_id)
        {
            byte[] buffer = new byte[BUFFER_MAX_SIZE];
            int packetLen;

            BasePacketHeader header = new BasePacketHeader(PacketDefine.TCP_PACKET_START_CODE, Convert.ToUInt32(Marshal.SizeOf(typeof(SetClntIDData))), PacketDefine.HANDLER_GAMEROOM, PacketDefine.HANDLER_GAMEROOM_SETCLNTID, 0, 0);
            BasePacketTrailer trailer = new BasePacketTrailer(PacketDefine.TCP_PACKET_END_CODE);
            SetClntIDData data = new SetClntIDData(clnt_id);

            ReturnRoomData retData;

            packetLen = PacketUtility.MakePacket(buffer, header, data, trailer);
            stream.Write(buffer, 0, packetLen);

            if (ReadData(buffer, out retData) < 0)
                return -1;

            if (retData.isSuccess == 0)
                return -1;

            return 0;
        }

        private RoomDatas SendGetGameRoomPacket(int offset)
        {
            byte[] buffer = new byte[BUFFER_MAX_SIZE];
            int packetLen;

            BasePacketHeader header = new BasePacketHeader(PacketDefine.TCP_PACKET_START_CODE, Convert.ToUInt32(Marshal.SizeOf(typeof(GetRoomData))), PacketDefine.HANDLER_GAMEROOM, PacketDefine.HANDLER_GAMEROOM_GET, 0, 0);
            BasePacketTrailer trailer = new BasePacketTrailer(PacketDefine.TCP_PACKET_END_CODE);
            GetRoomData data = new GetRoomData(offset);

            RoomDatas retData;

            packetLen = PacketUtility.MakePacket(buffer, header, data, trailer);
            stream.Write(buffer, 0, packetLen);
            if (ReadData(buffer, out retData) < 0)
                return retData;

            return retData;
        }

        private int SendCreateGameRoomPacket(int roomID)
        {
            byte[] buffer = new byte[BUFFER_MAX_SIZE];
            int packetLen;

            BasePacketHeader header = new BasePacketHeader(PacketDefine.TCP_PACKET_START_CODE, Convert.ToUInt32(Marshal.SizeOf(typeof(CreateRoomData))), PacketDefine.HANDLER_GAMEROOM, PacketDefine.HANDLER_GAMEROOM_CREATE, 0, 0);
            BasePacketTrailer trailer = new BasePacketTrailer(PacketDefine.TCP_PACKET_END_CODE);
            CreateRoomData data = new CreateRoomData(roomID);

            ReturnRoomData retData;

            packetLen = PacketUtility.MakePacket(buffer, header, data, trailer);
            stream.Write(buffer, 0, packetLen);
            if (ReadData(buffer, out retData) < 0)
                return -1;

            if (retData.isSuccess == 0)
                return -1;

            return 0;
        }

        private int SendJoinGameRoomPacket(int roomID)
        {
            byte[] buffer = new byte[BUFFER_MAX_SIZE];
            int packetLen;

            BasePacketHeader header = new BasePacketHeader(PacketDefine.TCP_PACKET_START_CODE, Convert.ToUInt32(Marshal.SizeOf(typeof(JoinRoomData))), PacketDefine.HANDLER_GAMEROOM, PacketDefine.HANDLER_GAMEROOM_JOIN, 0, 0);
            BasePacketTrailer trailer = new BasePacketTrailer(PacketDefine.TCP_PACKET_END_CODE);
            JoinRoomData data = new JoinRoomData(roomID);

            ReturnRoomData retData;

            packetLen = PacketUtility.MakePacket(buffer, header, data, trailer);
            stream.Write(buffer, 0, packetLen);
            if (ReadData(buffer, out retData) < 0)
                return -1;

            if (retData.isSuccess == 0)
                return -1;

            return 0;
        }

        private int SendOutGameRoomPacket(int roomID)
        {
            byte[] buffer = new byte[BUFFER_MAX_SIZE];
            int packetLen;

            BasePacketHeader header = new BasePacketHeader(PacketDefine.TCP_PACKET_START_CODE, Convert.ToUInt32(Marshal.SizeOf(typeof(OutRoomData))), PacketDefine.HANDLER_GAMEROOM, PacketDefine.HANDLER_GAMEROOM_OUT, 0, 0);
            BasePacketTrailer trailer = new BasePacketTrailer(PacketDefine.TCP_PACKET_END_CODE);
            OutRoomData data = new OutRoomData(roomID);

            ReturnRoomData retData;

            packetLen = PacketUtility.MakePacket(buffer, header, data, trailer);
            stream.Write(buffer, 0, packetLen);
            if (ReadData(buffer, out retData) < 0)
                return -1;

            if (retData.isSuccess == 0)
                return -1;

            return 0;
        }

        private void SendUpdateGameRoomPacket(GameRoomInfo updateInfo)
        {
            byte[] buffer = new byte[BUFFER_MAX_SIZE];
            int packetLen;

            BasePacketHeader header = new BasePacketHeader(PacketDefine.TCP_PACKET_START_CODE, Convert.ToUInt32(Marshal.SizeOf(typeof(UpdateRoomData))), PacketDefine.HANDLER_GAMEROOM, PacketDefine.HANDLER_GAMEROOM_UPDATE, 0, 0);
            BasePacketTrailer trailer = new BasePacketTrailer(PacketDefine.TCP_PACKET_END_CODE);
            UpdateRoomData data = new UpdateRoomData(updateInfo);

            packetLen = PacketUtility.MakePacket(buffer, header, data, trailer);
            stream.Write(buffer, 0, packetLen);
        }

        private int ReadData<T>(byte[] buffer, out T data)
        {
            BasePacketHeader header;
            BasePacketTrailer trailer;

            //Todo: 링버퍼 구현하면 바꾸기
            int packetSize = Marshal.SizeOf(typeof(BasePacketHeader)) + Marshal.SizeOf(typeof(T)) + Marshal.SizeOf(typeof(BasePacketTrailer));
            int readLen = 0;
            int offset = 0;

            for (int i = 0; i < 5; i++)
            {
                if ((readLen += stream.Read(buffer, readLen, packetSize - readLen)) >= packetSize)
                        break;
                if (i == 4)
                {
                    data = default(T);
                    return -1;
                }
            }

            offset += PacketUtility.ByteToObject(buffer, offset, out header);
            offset += PacketUtility.ByteToObject(buffer, offset, out data);
            offset += PacketUtility.ByteToObject(buffer, offset, out trailer);

            if ((CheckHeader(header) < 0) || (CheckTrailer(trailer) < 0))
                return -1;

            return 0;
        }

        private int CheckHeader(BasePacketHeader header)
        {
            if (header.startCode != PacketDefine.TCP_PACKET_START_CODE)
                return -1;
            return 0;
        }

        private int CheckTrailer(BasePacketTrailer trailer)
        {
            if (trailer.endCode != PacketDefine.TCP_PACKET_END_CODE)
                return -1;
            return 0;
        }
    }
}
