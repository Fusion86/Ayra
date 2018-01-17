﻿using Ayra.Core.Helpers;
using Ayra.Core.Models.WUP;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Ayra.Core.Classes
{
    public class NUSClientWiiU : NUSClient
    {
        protected override string nusBaseUrl => "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";
        protected override string nusUserAgent => "wii libnup/1.0";
        protected override Type TMDType => typeof(TMD);

        /// <summary>
        /// Download title to path
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        public override async Task DownloadTitle(dynamic tmd, string outDir)
        {
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

            for (int i = 0; i < tmd.Header.ContentCount; i++)
            {
                await DownloadContent(tmd, outDir, i);
            }
        }

        /// <summary>
        /// Download content
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        /// <param name="i">Content index</param>
        public override async Task DownloadContent(dynamic tmd, string outDir, int i)
        {
            NUSWebClient client = GetNewNUSWebClient();
            Debug.WriteLine($"[DownloadTitle] Downloading {i + 1}/{tmd.Header.ContentCount} {Utility.GetSizeString((long)tmd.Contents[i].Size)}");
            string titleId = tmd.Header.TitleId.ToString("X16");
            string url = nusBaseUrl + titleId + "/" + tmd.Contents[i].ContentId.ToString("X8");
            await client.DownloadFileTaskAsync(url, Path.Combine(outDir, tmd.Contents[i].ContentId.ToString("X8")));
        }
    }
}
