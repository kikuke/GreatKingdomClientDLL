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
    public struct GameRoomInfo
    {
        public int roomID;
        public int player_num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] playerID;
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
    public struct SetClntIDData
    {
        Int32 clnt_id;

        public SetClntIDData(int clnt_id)
        {
            this.clnt_id = clnt_id;
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
}
