using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET
{
    public static class Utils
    {
        public static byte[] AsLittleEndian(this byte[] bytes)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }


        public static byte[] ToByteArray(this string value)
        {
            List<byte> bytes = new List<byte>();

            foreach (char c in value)
                bytes.Add((byte)c);

            return bytes.ToArray();
        }
    }
}
