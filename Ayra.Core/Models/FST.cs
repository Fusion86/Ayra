using Ayra.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ayra.Core.Models
{
    // See http://wiiubrew.org/wiki/FST

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct FST_Header
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 Magic;

        [Endian(Endianness.BigEndian)]
        public UInt32 SecondaryHeaderSize; // Not sure

        [Endian(Endianness.BigEndian)]
        public UInt32 SecondaryHeaderCount;

        [Endian(Endianness.BigEndian)]
        public UInt16 Unk1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2; // Null bytes
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct FST_SecondaryHeader
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 Unk1;

        [Endian(Endianness.BigEndian)]
        public UInt32 Unk2;

        [Endian(Endianness.BigEndian)]
        public UInt64 TitleId;

        [Endian(Endianness.BigEndian)]
        public UInt32 GroupId;

        [Endian(Endianness.BigEndian)]
        public UInt16 Unk3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk4; // Null bytes
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FST_FDEntry
    {
        public byte Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] NameOffset;

        [Endian(Endianness.BigEndian)]
        public UInt32 Offset;

        [Endian(Endianness.BigEndian)]
        public UInt32 Size;

        [Endian(Endianness.BigEndian)]
        public UInt16 Flags;

        [Endian(Endianness.BigEndian)]
        public UInt16 StorageClusterIndex;
    }

    public class FST
    {
        public static FST Load(byte[] data)
        {

        }
    }
}
