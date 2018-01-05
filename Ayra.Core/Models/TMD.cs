using Ayra.Core.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
    public struct TMD_ContentRecord
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

    public class TMD
    {
        public TMD_Header Header;
        public TMD_ContentRecord[] Contents; // Count = Header.NumContents

        public static TMD Load(byte[] data)
        {
            TMD tmd = new TMD();
            tmd.Header = data.ToStruct<TMD_Header>();
            tmd.Contents = new TMD_ContentRecord[tmd.Header.NumContents];

            for (int i = 0; i < tmd.Header.NumContents; i++)
            {
                int offset = 0x30 * i;
                byte[] contentData = new byte[0x24];
                Array.Copy(data, 0xB04 + offset, contentData, 0, 0x24); // 0xB04 = sizeof(TMD_Header)
                TMD_ContentRecord contentRecord = contentData.ToStruct<TMD_ContentRecord>();
                tmd.Contents[i] = contentRecord;
            }

            return tmd;
        }
    }
}
