using Ayra.Core.Extensions;
using System;
using System.Runtime.InteropServices;

namespace Ayra.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMD_Header
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
    public unsafe struct TMD_ContentRecord
    {
        public UInt32 ContentId;
        public UInt16 Index;
        public UInt16 Type;
        public UInt64 Size;
        public fixed byte Hash[20];
    }

    public class TitleMetaData
    {
        public TMD_Header Header;
        public TMD_ContentRecord[] Contents; // Count = Header.NumContents

        public static TitleMetaData Load(byte[] data)
        {
            TitleMetaData tmd = new TitleMetaData();
            tmd.Header = data.ToStruct<TMD_Header>();

            return tmd;
        }
    }
}
