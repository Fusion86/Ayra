﻿// Based on https://github.com/crediar/cdecrypt/blob/master/main.cpp

using Ayra.Core.Data;
using Ayra.Core.Logging;
using Ayra.Core.Models.WUP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Ayra.Core.Helpers.WUP
{
    public class CDecrypt
    {
        private static readonly ILog Logger = LogProvider.For<CDecrypt>();

        public static void DecryptContents(TMD tmd, Ticket ticket, string path)
        {
            Logger.Info("TMD version: " + tmd.Header.Version);
            if (tmd.Header.Version != 1) throw new NotSupportedException();

            Logger.Info("Title version: " + tmd.Header.TitleVersion);
            Logger.Info("Content count: " + tmd.Header.ContentCount);

            //
            // AES setup
            //

            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            switch (tmd.Header.Issuer)
            {
                case "Root-CA00000003-CP0000000b":
                    aes.Key = KeyChain.WIIU_COMMON_KEY;
                    break;

                case "Root-CA00000004-CP00000010":
                    aes.Key = KeyChain.WIIU_COMMON_DEV_KEY;
                    break;

                default: throw new NotSupportedException("Unknown Root type: " + tmd.Header.Issuer);
            }

            //
            // Decrypt title key
            //

            byte[] encryptedTitleKey = ticket.Tickets[0].Header.EncryptedTitleKey;
            byte[] decryptedTitleKey = new byte[16];

            // First 8 bytes are titleId and the last 8 bytes are 0x00
            byte[] iv = BitConverter.GetBytes(tmd.Header.TitleId);
            if (BitConverter.IsLittleEndian) Array.Reverse(iv);
            Array.Resize(ref iv, 16); // New space will have 0x00 as value

            aes.IV = iv;

            using (MemoryStream ms = new MemoryStream(encryptedTitleKey))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
            {
                cs.Read(decryptedTitleKey, 0, decryptedTitleKey.Length);
            }

            //
            // Read Contents[0]
            //

            string cntName = tmd.Contents[0].ContentId.ToString("X08");
            byte[] encryptedContent = File.ReadAllBytes(Path.Combine(path, cntName));
            byte[] decryptedContent = new byte[encryptedContent.Length];

            if ((int)tmd.Contents[0].Size != encryptedContent.Length) throw new Exception("Size of Contents[0] is wrong!");

            aes.Key = decryptedTitleKey;
            aes.IV = new byte[16]; // 0x00 * 16

            using (MemoryStream ms = new MemoryStream(encryptedContent))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
            {
                cs.Read(decryptedContent, 0, encryptedContent.Length);
            }

            Task.Factory.StartNew(() =>
            {
                File.WriteAllBytes(Path.Combine(path, cntName + ".dec"), decryptedContent);
            });

            FST fst = FST.Load(decryptedContent);

            // Extract
            // Wip stuff, I still don't even know how the original function works
            for (int i = 0; i < fst.Entries.Length; i++)
            {
                const int BLOCK_SIZE = 0x8000;

                // TODO: Maybe `typeof(entry) == FST_FileInfo` is faster?
                FST_FileInfo entry = fst.Entries[i] as FST_FileInfo;
                if (entry == null) continue;
                if (Convert.ToBoolean(entry.Flags & 0x80)) continue;

                UInt32 contentId = tmd.Contents[entry.StorageClusterIndex].ContentId;
                string contentName = contentId.ToString("X08");
                string contentPath = Path.Combine(path, contentName);
                string outName = fst.FilePaths[i]; // Relative path (as seen from the archive root)
                string outPath = Path.Combine(path, outName);

                int blockNr = (int)(entry.FileOffset / (uint)BLOCK_SIZE);

                UInt64 realOffset = entry.FileOffset / BLOCK_SIZE * BLOCK_SIZE;
                UInt64 soffset = entry.FileOffset - realOffset; // What is this?

                Array.Clear(aes.IV, 0, aes.IV.Length);
                aes.IV[1] = (byte)entry.StorageClusterIndex;
                // aes.Key is unchanged, but still holds the old (needed) value

                using (FileStream contentStream = new FileStream(contentPath, FileMode.Open))
                using (FileStream outStream = new FileStream(outPath, FileMode.Create))
                {
                    // cdecrypt-2 Size = entry.FileSize
                }
            }
        }

        public static string GetTitleIdForContent(string path)
        {
            // Idk if this is even possible
            throw new NotImplementedException();
        }

        public static void DownloadMissingFiles(string path)
        {
            Lazy<string> titleId = new Lazy<string>(() => GetTitleIdForContent("download/00something"));
            IEnumerable<string> files = new DirectoryInfo(path).GetFiles().Select(x => x.Name);

            if (!files.Contains("tmd"))
            {
                // Download
            }

            if (!files.Contains("cetk"))
            {
                // Download
            }
        }
    }
}
