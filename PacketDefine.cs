using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatKingdomClient
{
    public class PacketDefine
    {
        public const int TCP_PACKET_START_CODE = 0x77;
        public const int TCP_PACKET_END_CODE = 0x33;

        public const int HANDLER_GAMEROOM = 0x02;
        public const int HANDLER_GAMEROOM_SETCLNTID = 0x00;
        public const int HANDLER_GAMEROOM_RETURN = 0x01;
        public const int HANDLER_GAMEROOM_GET = 0x02;
        public const int HANDLER_GAMEROOM_GETRETURN = 0x0F;
        public const int HANDLER_GAMEROOM_CREATE = 0x03;
        public const int HANDLER_GAMEROOM_JOIN = 0x04;
        public const int HANDLER_GAMEROOM_OUT = 0x05;
        public const int HANDLER_GAMEROOM_UPDATE = 0x06;
    }
}
