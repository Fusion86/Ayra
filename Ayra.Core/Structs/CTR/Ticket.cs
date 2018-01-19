using Ayra.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ayra.Core.Structs.CTR
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ticket
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
        public string Issuer; // Root-CA%08x-CP%08x
    }
}
