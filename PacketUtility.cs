using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GreatKingdomClient
{
    public class PacketUtility
    {
        public static int MakePacket<T>(byte[] packet, BasePacketHeader header, T payLoad, BasePacketTrailer trailer)
        {
            int packetSize = 0;

            packetSize += ObjcetToByte(packet, packetSize, header);
            packetSize += ObjcetToByte(packet, packetSize, payLoad);
            packetSize += ObjcetToByte(packet, packetSize, trailer);
            return packetSize;
        }

        public static int ObjcetToByte<T>(byte[] buffer, int offset, T data)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(data, ptr, true);
            Marshal.Copy(ptr, buffer, offset, size);
            Marshal.FreeHGlobal(ptr);

            return size;
        }
        public static int ByteToObject<T>(byte[] buffer, int offset, out T data)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, offset, ptr, size);
            data = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return size;
        }
    }
}
