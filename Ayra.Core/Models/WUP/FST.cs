using Ayra.Core.Extensions;
using Ayra.Core.Logging;
using Ayra.Core.Structs.WUP;
using System;
using System.IO;

namespace Ayra.Core.Models.WUP
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

    public class FST_FDInfoBase
    {
        protected _FST_FDInfo native;

        public bool IsDirectory => Convert.ToBoolean(native.Type & 1);

        public byte Type => native.Type;
        public ushort Flags => native.Flags;
        public ushort StorageClusterIndex => native.StorageClusterIndex; // ContentId

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

        public static FST_FDInfoBase Load(byte[] data)
        {
            FST_FDInfoBase info = new FST_FDInfoBase { native = data.ToStruct<_FST_FDInfo>() };
            if (info.IsDirectory) return new FST_DirectoryInfo { native = info.native };
            else return new FST_FileInfo { native = info.native };
        }
    }

    public class FST_DirectoryInfo : FST_FDInfoBase
    {
        public UInt32 ParentOffset => native.Offset;

        //public UInt32 NextOffset => native.Size;
        public UInt32 FileCount => native.Size;
    }

    public class FST_FileInfo : FST_FDInfoBase
    {
        public UInt32 FileOffset => native.Offset;
        public UInt32 FileSize => native.Size;
    }

    public class FST
    {
        private static readonly ILog Logger = LogProvider.For<FST>();

        public FST_Header Header;
        public FST_SecondaryHeader[] SecondaryHeaders;
        public FST_FDInfoBase[] Entries;
        public string[] FilePaths; // And directory path

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
            FST_DirectoryInfo rootEntry = FST_FDInfoBase.Load(rootData) as FST_DirectoryInfo;

            Logger.Info($"Found {rootEntry.FileCount} files/directories");

            // Read all entries (including root again)
            fst.Entries = new FST_FDInfoBase[rootEntry.FileCount];
            fst.Entries[0] = rootEntry;
            for (int i = 1; i < rootEntry.FileCount; ++i)
            {
                byte[] entryData = new byte[0x10]; // NOTE: Look carefully ++i instead of i++
                Buffer.BlockCopy(data, offset + i * 0x10, entryData, 0, entryData.Length);
                fst.Entries[i] = FST_FDInfoBase.Load(entryData);
            }

            #region Build file paths

            // TODO: Rewrite this

            int nameTableOffset = offset + (int)rootEntry.FileCount * 0x10; // 0x10 = sizeof(FST_FileInfo)

            int[] entry = new int[16];
            int[] lentry = new int[16];

            int level = 0; // Max is 15 (aka 16 states)
            string[] fileNames = new string[rootEntry.FileCount];
            fst.FilePaths = new string[rootEntry.FileCount];

            for (int i = 1; i < rootEntry.FileCount; ++i)
            {
                while (level > 0 && lentry[level - 1] == i) level--;

                char[] szName = new char[0xFF];
                for (int j = 0; j < szName.Length; j++)
                {
                    szName[j] = (char)data[nameTableOffset + fst.Entries[i].NameOffset + j];
                    if (szName[j] == 0x00) break; // We are reading zero terminated strings, so break if we find the string terminator (0x00)
                }

                fileNames[i] = new string(szName).Split('\0')[0]; // Split to remove zero terminators from string

                if (fst.Entries[i].IsDirectory)
                {
                    entry[level] = i;
                    lentry[level++] = (int)(fst.Entries[i] as FST_DirectoryInfo).FileCount;

                    if (level > 15)
                    {
                        throw new Exception("Level error");
                    }

                    Logger.Debug($"Found directory '{fileNames[i]}' at level {level}");
                }

                string path = "";
                for (int j = 0; j < level; j++)
                {
                    int dirnameIndex = entry[j];
                    path += fileNames[dirnameIndex] + Path.DirectorySeparatorChar;
                }

                if (!fst.Entries[i].IsDirectory) path += fileNames[i];
                fst.FilePaths[i] = path;

                string type = fst.Entries[i].IsDirectory ? "directory" : "file";
                Logger.Debug($"Found {type} '{path}'");
            }

            #endregion Build file paths

            return fst;
        }
    }
}
