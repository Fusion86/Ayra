﻿using Ayra.Core.Helpers;
using Ayra.Core.Models.WiiU;
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

        /// <summary>
        /// Download title to path, but now in parallel
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        /// <returns></returns>
        public async Task DownloadTitleParallel(dynamic tmd, string outDir, int maxConcurrent = 4)
        {
            throw new NotSupportedException();

            //List<Task> tasks = new List<Task>();
            //for (int i = 0; i < tmd.Header.ContentCount; i++)
            //{
            //    int wtf = i; // TODO: If we don't do this then I __assume__ it'll pass the value of i at the end of this for loop, aka tmd.Header.NumContents
            //    tasks.Add(new Task(async () => await DownloadContent(tmd, outDir, wtf)));
            //}

            //// FIXME: Doesn't wait
            //await Tasks.StartAndWaitAllThrottledAsync(tasks, maxConcurrent);
        }
    }
}
