using Ayra.Core.Extensions;
using Ayra.Core.Structs;
using System;
using System.Diagnostics;

namespace Ayra.Core.Models
{
    public class FST_Header
    {
        private _FST_Header native;

        public UInt32 SecondaryHeaderSize => native.SecondaryHeaderSize;
        public UInt32 SecondaryHeaderCount => native.SecondaryHeaderCount;

        public static FST_Header Load(byte[] data)
        {
            return new FST_Header
            {
                native = data.ToStruct<_FST_Header>()
            };
        }
    }

    public class FST_SecondaryHeader
    {
        private _FST_SecondaryHeader native;

        public static FST_SecondaryHeader Load(byte[] data)
        {
            return new FST_SecondaryHeader
            {
                native = data.ToStruct<_FST_SecondaryHeader>()
            };
        }
    }

    public class FST_FileInfo
    {
        private _FST_FileInfo native;

        public bool IsDirectory => Convert.ToBoolean(native.Type & 1);
        public int NameOffset 
        {
            get
            {
                byte[] data = new byte[4];
                Buffer.BlockCopy(native.NameOffset, 0, data, 1, native.NameOffset.Length);
                Array.Reverse(data);
                return BitConverter.ToInt32(data, 0);
            }
        }

        public UInt32 FileCount => native.Size;
        public UInt32 FileSize => native.Size;

        public static FST_FileInfo Load(byte[] data)
        {
            return new FST_FileInfo
            {
                native = data.ToStruct<_FST_FileInfo>()
            };
        }
    }

    public class FST
    {
        public FST_Header Header;
        public FST_SecondaryHeader[] SecondaryHeaders;
        public FST_FileInfo[] Entries;
        public string[] FileNames; // Or directory

        public static FST Load(byte[] data)
        {
            FST fst = new FST();
            fst.Header = FST_Header.Load(data);
            fst.SecondaryHeaders = new FST_SecondaryHeader[fst.Header.SecondaryHeaderCount];

            // 0x20 = sizeof(FST_Header)  and  sizeof(FST_SecondaryHeader)
            // fst.Header.SecondaryHeaderSize is always 0x20 as far as I know

            int offset = 0x20;
            for (int i = 0; i < fst.Header.SecondaryHeaderCount; i++)
            {
                byte[] secondaryHeaderData = new byte[0x20];
                Buffer.BlockCopy(data, offset, secondaryHeaderData, 0, secondaryHeaderData.Length);
                fst.SecondaryHeaders[i] = FST_SecondaryHeader.Load(secondaryHeaderData);
                offset += 0x20;
            }

            // Read root
            byte[] rootData = new byte[0x10];
            Buffer.BlockCopy(data, offset, rootData, 0, rootData.Length);
            FST_FileInfo rootEntry = FST_FileInfo.Load(rootData);

            Debug.WriteLine($"[FST] Found {rootEntry.FileCount} files/directories");

            // Read all entries (including root again)
            fst.Entries = new FST_FileInfo[rootEntry.FileCount];
            for (int i = 0; i < rootEntry.FileCount; ++i)
            {
                byte[] entryData = new byte[0x10]; // NOTE: Look carefully ++i instead of i++
                Buffer.BlockCopy(data, offset + i * 0x10, entryData, 0, entryData.Length);
                fst.Entries[i] = FST_FileInfo.Load(entryData);
            }

            // fst.Entries[0] == rootEntry, so they are the same (not just the same data, but same address)

            // Read name table
            int nameTableOffset = offset + (int)rootEntry.FileCount * 0x10; // 0x10 = sizeof(FST_FileInfo)

            UInt32[] entry = new UInt32[16];
            UInt32[] lentry = new UInt32[16];

            int level = 0; // Max is 15 (aka 16 states)
            fst.FileNames = new string[rootEntry.FileCount];
            for (int i = 0; i < rootEntry.FileCount; i++)
            {
                char[] chars = new char[0xFF];
                for (int j = 0; j < chars.Length; j++)
                {
                    chars[j] = (char)data[nameTableOffset + fst.Entries[i].NameOffset + j];
                    if (chars[j] == 0x00) break;
                }

                string name = new string(chars).Split('\0')[0]; // Split to remove zero terminators from string
                fst.FileNames[i] = name;

                if (fst.Entries[i].IsDirectory)
                {
                    Debug.WriteLine($"[FST] Found directory '{name}' at level {0}");
                }
                else
                {
                    Debug.WriteLine($"[FST] Found file '{name}' at level {0}");
                }
            }

            return fst;
        }
    }
}
