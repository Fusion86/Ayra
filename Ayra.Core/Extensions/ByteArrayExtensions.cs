using System;
using System.Runtime.InteropServices;

namespace Ayra.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public static T ToStruct<T>(this byte[] bytes)
        {
            EndiannessHelper.RespectEndianness(typeof(T), bytes);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
