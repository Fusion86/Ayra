using Ayra.Core.Enums;
using Ayra.Core.Helpers;
using Ayra.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Ayra.Core.Classes
{
    public class NUSClient
    {
        private const string WIIU_NUS_URL = "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";
        private const string WIIU_USER_AGENT = "wii libnup/1.0";

        private readonly string nusBaseUrl;
        private readonly string nusUserAgent;

        public DownloadProgressChangedEventHandler DownloadProgressChangedEventHandler;

        public NUSClient(NDevice device)
        {
            switch (device)
            {
                case NDevice.DSI:
                    throw new NotImplementedException();

                case NDevice.WII:
                    throw new NotImplementedException();

                case NDevice.WII_U:
                    nusBaseUrl = WIIU_NUS_URL;
                    nusUserAgent = WIIU_USER_AGENT;
                    break;
            }
        }

        private NUSWebClient GetNewNUSWebClient()
        {
            return new NUSWebClient { UserAgent = nusUserAgent };
        }

        /// <summary>
        /// Download TitleMetaData
        /// </summary>
        /// <param name="titleId">Title to download the metadata for</param>
        /// <returns></returns>
        public async Task<TMD> DownloadTMD(string titleId, bool saveLocal = false, string savePath = "tmd")
        {
            NUSWebClient client = GetNewNUSWebClient();
            string url = nusBaseUrl + titleId + "/tmd";
            byte[] data = await client.DownloadDataTaskAsync(new Uri(url));
            if (saveLocal) File.WriteAllBytes(savePath, data);
            return TMD.Load(ref data);
        }

        /// <summary>
        /// Download Common E-Ticket
        /// </summary>
        /// <param name="titleId"></param>
        /// <returns></returns>
        public async Task<object> DownloadCetk(string titleId, bool saveLocal = false, string savePath = "cetk")
        {
            string url = nusBaseUrl + titleId + "/cetk";
            throw new NotImplementedException();
            //if (saveLocal) File.WriteAllBytes(savePath, data);
        }

        /// <summary>
        /// Download title to path
        /// </summary>
        /// <param name="titleId"></param>
        public async Task DownloadTitle(string titleId, string path)
        {
            Debug.WriteLine($"[DownloadTitle] Downloading TMD for TitleID {titleId}");
            TMD tmd = await DownloadTMD(titleId);
            await DownloadTitle(tmd, path);
        }

        /// <summary>
        /// Download title to path
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        public async Task DownloadTitle(TMD tmd, string outDir)
        {
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

            for (int i = 0; i < tmd.Header.NumContents; i++)
            {
                await DownloadContent(tmd, outDir, i);
            }
        }

        /// <summary>
        /// Download title to path, but now in parallel
        /// </summary>
        /// <param name="titleId"></param>
        public async Task DownloadTitleParallel(string titleId, string path, int maxConcurrent = 4)
        {
            Debug.WriteLine($"[DownloadTitle] Downloading TMD for TitleID {titleId}");
            TMD tmd = await DownloadTMD(titleId);
            await DownloadTitleParallel(tmd, path, maxConcurrent);
        }

        /// <summary>
        /// Download title to path, but now in parallel
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        /// <returns></returns>
        public async Task DownloadTitleParallel(TMD tmd, string outDir, int maxConcurrent = 4)
        {
            throw new NotSupportedException("Doesn't work atm");

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < tmd.Header.NumContents; i++)
            {
                int wtf = i; // TODO: If we don't do this then I __assume__ it'll pass the value of i at the end of this for loop, aka tmd.Header.NumContents
                tasks.Add(new Task(async () => await DownloadContent(tmd, outDir, wtf)));
            }


            // FIXME: Doesn't wait
            await Tasks.StartAndWaitAllThrottledAsync(tasks, maxConcurrent);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        /// <param name="i">Content index</param>
        private async Task DownloadContent(TMD tmd, string outDir, int i)
        {
            NUSWebClient client = GetNewNUSWebClient();
            Debug.WriteLine($"[DownloadTitle] Downloading {i + 1}/{tmd.Header.NumContents} {Utility.GetSizeString((long)tmd.Contents[i].Size)}");
            string titleId = tmd.Header.TitleId.ToString("X16");
            string url = nusBaseUrl + titleId + "/" + tmd.Contents[i].ContentId.ToString("X8");
            await client.DownloadFileTaskAsync(url, Path.Combine(outDir, tmd.Contents[i].ContentId.ToString("X8")));
        }
    }
}
