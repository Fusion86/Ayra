// Based on https://github.com/crediar/cdecrypt/blob/master/main.cpp

using Ayra.Core.Models;
using Ayra.Core.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Ayra.Core.Data;

namespace Ayra.Core.Helpers
{
    public static class CDecrypt
    {
        public static void DecryptContents(TMD tmd, byte[] encTitleKey, string path)
        {
            if (encTitleKey.Length != 16) throw new Exception("Encrypted title key has to be 16 bytes!");

            Debug.WriteLine("[DecryptContents] TMD version: " + tmd.Header.Version);
            if (tmd.Header.Version != 1) throw new NotSupportedException();

            Debug.WriteLine("[DecryptContents] Title version: " + tmd.Header.TitleVersion);
            Debug.WriteLine("[DecryptContents] Content count: " + tmd.Header.NumContents);

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
            
            byte[] decTitleKey = new byte[16];

            // First 8 bytes are titleId and the last 8 bytes are 0x00
            byte[] iv = BitConverter.GetBytes(tmd.Header.TitleId);
            if (BitConverter.IsLittleEndian) Array.Reverse(iv);
            Array.Resize(ref iv, 16); // New space will have 0x00 as value

            aes.IV = iv;

            using (MemoryStream ms = new MemoryStream(encTitleKey))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
            {
                cs.Read(decTitleKey, 0, decTitleKey.Length);
            }

            //
            // Stuff
            //

            aes.IV = new byte[16]; // 0x00 * 16

            string appName = tmd.Contents[0].ContentId.ToString("X08") + ".app";
        }
    }
}