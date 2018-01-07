using System;
using System.Collections.Generic;
using System.Text;

namespace Ayra.Core.Helpers
{
    public static class Memory
    {
        public unsafe static void Memset(byte* dest, byte value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                *(dest + i) = value;
            }
        }

        public unsafe static void Memcpy(byte* dest, byte* source, int count)
        {

        }
    }
}
