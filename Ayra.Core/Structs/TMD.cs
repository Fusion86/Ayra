using Ayra.Core.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ayra.Core.Structs
{
    // Same for CTR and WUP

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct _TMD_ContentRecord
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 ContentId;

        [Endian(Endianness.BigEndian)]
        public UInt16 Index;

        [DebuggerDisplay("{Type,h}")]
        [Endian(Endianness.BigEndian)]
        public UInt16 Type;

        [Endian(Endianness.BigEndian)]
        public UInt64 Size;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.U1)]
        public byte[] Hash;
    }
}
