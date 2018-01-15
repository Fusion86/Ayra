﻿using Ayra.Core.Extensions;
using Ayra.Core.Structs;
using System;

namespace Ayra.Core.Models
{
    /// <summary>
    /// TMD container that works for 3DS and Wii U games
    /// </summary>
    public class TMD
    {
        public _TMD_Header Header;
        public _TMD_ContentRecord[] Contents; // Count = Header.NumContents

        public static TMD Load(ref byte[] data)
        {
            TMD tmd = new TMD();
            tmd.Header = data.ToStruct<_TMD_Header>();
            tmd.Contents = new _TMD_ContentRecord[tmd.Header.NumContents];

            for (int i = 0; i < tmd.Header.NumContents; i++)
            {
                int offset = 0x30 * i;
                byte[] contentData = new byte[0x24];
                Buffer.BlockCopy(data, 0xB04 + offset, contentData, 0, contentData.Length); // 0xB04 = sizeof(TMD_Header)
                _TMD_ContentRecord contentRecord = contentData.ToStruct<_TMD_ContentRecord>();
                tmd.Contents[i] = contentRecord;
            }

            return tmd;
        }
    }
}
