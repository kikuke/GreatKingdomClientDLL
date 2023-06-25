using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GreatKingdomClient
{

    public class NetworkClient
    {
        const int BUFFER_MAX_SIZE = 2048;

        private TcpClient tc;
        private NetworkStream stream;

        public NetworkClient(string serverIP, int serverPort)
        {
            tc = new TcpClient(serverIP, serverPort);
            stream = tc.GetStream();
        }

        public int SendSetClntIDPacket(int clnt_id)
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

        public int SendCreateGameRoomPacket(int roomID)
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

        public int SendJoinGameRoomPacket(int roomID)
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

        public int SendOutGameRoomPacket(int roomID)
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

        public void Close()
        {
            tc.Close();
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
