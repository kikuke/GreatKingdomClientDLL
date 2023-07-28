using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GreatKingdomClient
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReturnUserData
    {
        // if success 1, else 0
        public int isSuccess;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetClntIDData
    {
        public int clnt_id;

        public SetClntIDData(int clnt_id)
        {
            this.clnt_id = clnt_id;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GameRoomInfo
    {
        public int roomID;
        public int player_num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] playerID;

        //0 준비중, 1 시작, 2 종료 이런 느낌.
        public int roomStatus;

        //판 정보. 1차원 배열
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9*9)]
        public int[] panel;

        public int passNum;

        //두명 다 패스했는지
        public int isCSP1;
        public int isCSP2;

        public override string ToString()
        {
            return base.ToString() + ": " + "\n{\nroomID: " + roomID + "\nplayer_num: " + player_num + "\nplayer1 ID: " + playerID[0] + "\nplayer2 ID: " + playerID[1] + "\n}\n";
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomDatas
    {
        public int roomNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public GameRoomInfo[] roomInfo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReturnRoomData
    {
        public int isSuccess;

        public GameRoomInfo roomInfo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BasePacketHeader
    {
        public UInt32 startCode;
        UInt32 payloadLen;

        UInt32 mainOp;
        UInt32 subOP;

        UInt32 flag;
        UInt32 auth;

        public BasePacketHeader(uint startCode, uint payloadLen, uint mainOp, uint subOP, uint flag, uint auth)
        {
            this.startCode = startCode;
            this.payloadLen = payloadLen;
            this.mainOp = mainOp;
            this.subOP = subOP;
            this.flag = flag;
            this.auth = auth;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BasePacketTrailer
    {
        public UInt32 endCode;

        public BasePacketTrailer(uint endCode)
        {
            this.endCode = endCode;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GetRoomData
    {
        Int32 offset;

        public GetRoomData(int offset)
        {
            this.offset = offset;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateRoomData
    {
        Int32 roomID;

        public CreateRoomData(int roomID)
        {
            this.roomID = roomID;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct JoinRoomData
    {
        Int32 roomID;

        public JoinRoomData(int roomID)
        {
            this.roomID = roomID;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OutRoomData
    {
        Int32 roomID;

        public OutRoomData(int roomID)
        {
            this.roomID = roomID;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UpdateRoomData
    {
        public GameRoomInfo roomInfo;

        public UpdateRoomData(GameRoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
        }
    }
}
