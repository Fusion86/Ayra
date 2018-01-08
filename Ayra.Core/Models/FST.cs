using Ayra.Core.Extensions;
using Ayra.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public UInt32 SecondaryHeaderSize; // Not sure, usually 0x20

        [Endian(Endianness.BigEndian)]
        public UInt32 SecondaryHeaderCount; // Entries

        [Endian(Endianness.BigEndian)]
        public UInt16 Unk1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2; // Null bytes
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct FST_SecondaryHeader
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
    public struct FST_FileInfo
    {
        public byte Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] NameOffset;

        [Endian(Endianness.BigEndian)]
        public UInt32 Offset;

        [Endian(Endianness.BigEndian)]
        public UInt32 Size; // File size or directory size

        [Endian(Endianness.BigEndian)]
        public UInt16 Flags;

        [Endian(Endianness.BigEndian)]
        public UInt16 StorageClusterIndex;
    }

    public class FST
    {
        public FST_Header Header;
        public FST_SecondaryHeader[] SecondaryHeaders;
        public FST_FileInfo[] Entries;

        public static FST Load(byte[] data)
        {
            FST fst = new FST();
            fst.Header = data.ToStruct<FST_Header>();
            fst.SecondaryHeaders = new FST_SecondaryHeader[fst.Header.SecondaryHeaderCount];

            // 0x20 = sizeof(FST_Header)  and  sizeof(FST_SecondaryHeader)
            // fst.Header.SecondaryHeaderSize is always 0x20 as far as I know

            int offset = 0x20;
            for (int i = 0; i < fst.Header.SecondaryHeaderCount; i++)
            {
                byte[] secondaryHeaderData = new byte[0x20];
                Buffer.BlockCopy(data, offset, secondaryHeaderData, 0, secondaryHeaderData.Length);
                fst.SecondaryHeaders[i] = secondaryHeaderData.ToStruct<FST_SecondaryHeader>();
                offset += 0x20;
            }

            // Read root
            byte[] rootData = new byte[0x10];
            Buffer.BlockCopy(data, offset, rootData, 0, rootData.Length);
            FST_FileInfo rootEntry = rootData.ToStruct<FST_FileInfo>();

            Debug.WriteLine($"[FST] Found {rootEntry.Size} files/directories");

            // Read all entries (including root again)
            fst.Entries = new FST_FileInfo[rootEntry.Size];
            for (int i = 0; i < rootEntry.Size; ++i)
            {
                byte[] entryData = new byte[0x10]; // NOTE: Look carefully ++i instead of i++
                Buffer.BlockCopy(data, offset + i * 0x10, entryData, 0, entryData.Length);
                fst.Entries[i] = entryData.ToStruct<FST_FileInfo>();
            }

            // fst.Entries[0] == rootEntry, so they are the same (not just the same data, but same address)

            // Read name table
            int nameTableOffset = offset + (int)rootEntry.Size * 0x10; // 0x10 = sizeof(FST_FileInfo)

            // TODO: Stuff

            return fst;
        }
    }
}
