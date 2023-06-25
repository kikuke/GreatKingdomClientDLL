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
    }
}
