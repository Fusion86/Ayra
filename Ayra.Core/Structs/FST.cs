using Ayra.Core.Extensions;
using Ayra.Core.Helpers;
using System;
using System.Runtime.InteropServices;

namespace Ayra.Core.Structs
{
    // See http://wiiubrew.org/wiki/FST

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct _FST_Header // cdecrypt: FST (first part)
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 Magic;

        [Endian(Endianness.BigEndian)]
        public UInt32 SecondaryHeaderSize; // Not sure, usually 0x20

        [Endian(Endianness.BigEndian)]
        public UInt32 SecondaryHeaderCount; // Entries

        [Endian(Endianness.BigEndian)]
        public UInt16 Unk1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2; // Null bytes
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct _FST_SecondaryHeader // cdecrypt: FSTInfo
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 Unk1; // Starting cluster?

        [Endian(Endianness.BigEndian)]
        public UInt32 Unk2; // Clusters between repeats?

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
    public struct _FST_FDInfo // cdecrypt: FEntry
    {
        public byte Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] NameOffset;

        [Endian(Endianness.BigEndian)]
        public UInt32 Offset; // FileOffset or ParentOffset

        [Endian(Endianness.BigEndian)]
        public UInt32 Size; // FileLength or NextOffset

        [Endian(Endianness.BigEndian)]
        public UInt16 Flags;

        [Endian(Endianness.BigEndian)]
        public UInt16 StorageClusterIndex; // Or maybe ContentId
    }
}
