using Ayra.Core.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ayra.Core.Structs.WiiU
{
    // See http://wiiubrew.org/wiki/Title_metadata

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _TMD_Header
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 SignatureType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U1)]
        public byte[] Signature;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60, ArraySubType = UnmanagedType.U1)]
        public byte[] Fill1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Issuer; // Root-CA%08x-CP%08x

        public byte Version;
        public byte CaCrlVersion;
        public byte SignerCrlVersion;
        public byte Fill2;

        [Endian(Endianness.BigEndian)]
        public UInt64 SysVersion;

        [Endian(Endianness.BigEndian)]
        public UInt64 TitleId;

        [Endian(Endianness.BigEndian)]
        public UInt32 TitleType;

        [Endian(Endianness.BigEndian)]
        public UInt16 GroupId; // Publisher

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 62, ArraySubType = UnmanagedType.U1)]
        public byte[] Reserved;

        [Endian(Endianness.BigEndian)]
        public UInt32 AccessRights;

        [Endian(Endianness.BigEndian)]
        public UInt16 TitleVersion;

        [Endian(Endianness.BigEndian)]
        public UInt16 NumContents;

        [Endian(Endianness.BigEndian)]
        public UInt16 BootIndex;

        [Endian(Endianness.BigEndian)]
        public UInt16 Fill3;
    }

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
