using Ayra.Core.Helpers;
using System;
using System.Runtime.InteropServices;

namespace Ayra.Core.Structs.CTR
{
    // See https://www.3dbrew.org/wiki/Title_metadata

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _TMD_Header
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
        public string Issuer; // Root-CA%08x-CP%08x

        public byte Version;
        public byte CaCrlVersion;
        public byte SignerCrlVersion;
        public byte Fill1;

        [Endian(Endianness.BigEndian)]
        public UInt64 SysVersion;

        [Endian(Endianness.BigEndian)]
        public UInt64 TitleId;

        [Endian(Endianness.BigEndian)]
        public UInt32 TitleType;

        [Endian(Endianness.BigEndian)]
        public UInt16 GroupId;

        [Endian(Endianness.BigEndian)]
        public UInt32 SaveDataSize; // bytes, Also SRL Public Save Data Size

        [Endian(Endianness.BigEndian)]
        public UInt32 SRLPrivateSaveDataSize; // bytes

        [Endian(Endianness.BigEndian)]
        public UInt32 Fill2;

        public byte SRLFlag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x31, ArraySubType = UnmanagedType.U1)]
        public byte[] Fill3;

        [Endian(Endianness.BigEndian)]
        public UInt32 AccessRights;

        [Endian(Endianness.BigEndian)]
        public UInt16 TitleVersion;

        [Endian(Endianness.BigEndian)]
        public UInt16 ContentCount;

        [Endian(Endianness.BigEndian)]
        public UInt16 BootContent;

        [Endian(Endianness.BigEndian)]
        public UInt16 Fill4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20, ArraySubType = UnmanagedType.U1)]
        public byte[] Hash; // SHA-256 Hash of the Content Info Records
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct _TMD_ContentInfoRecord
    {
        [Endian(Endianness.BigEndian)]
        public UInt16 ContentIndexOffset;

        [Endian(Endianness.BigEndian)]
        public UInt16 ContentCommandCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20, ArraySubType = UnmanagedType.U1)]
        public byte[] Hash; // SHA-256 hash of the next k content records that has not been hashed yet
    }
}
