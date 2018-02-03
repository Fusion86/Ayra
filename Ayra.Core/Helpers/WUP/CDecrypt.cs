// Based on https://github.com/crediar/cdecrypt/blob/master/main.cpp

using Ayra.Core.Data;
using Ayra.Core.Logging;
using Ayra.Core.Models.WUP;
using System;
using System.IO;
using System.Security.Cryptography;

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

            //if (Debugger.IsAttached) File.WriteAllBytes(Path.Combine(path, cntName + ".dec"), decryptedContent);

            FST fst = FST.Load(decryptedContent);
            int num = fst.Entries[0].NameOffset;
        }
    }
}
