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
    }
}
