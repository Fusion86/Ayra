using Ayra.Core.Helpers;
using Ayra.Core.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Ayra.Core.Structs;

namespace Ayra.Core.Classes
{
    public abstract class NUSClient
    {
        private static readonly ILog Logger = LogProvider.For<NUSClient>();

        protected abstract string nusBaseUrl { get; }
        protected abstract string nusUserAgent { get; }
        protected abstract Type TMDType { get; }

        protected NUSWebClient GetNewNUSWebClient()
        {
            return new NUSWebClient { UserAgent = nusUserAgent };
        }

        /// <summary>
        /// Download TitleMetaData
        /// </summary>
        /// <param name="titleId">Title to download the metadata for</param>
        /// <returns></returns>
        public async Task<dynamic> DownloadTMD(string titleId, bool saveLocal = false, string savePath = "tmd")
        {
            NUSWebClient client = GetNewNUSWebClient();
            string url = nusBaseUrl + titleId + "/tmd";
            byte[] data = await client.DownloadDataTaskAsync(new Uri(url));
            if (saveLocal) File.WriteAllBytes(savePath, data);
            return LoadTMD(data);
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
            Logger.Info($"Downloading TMD for TitleID {titleId}");
            dynamic tmd = await DownloadTMD(titleId);
            await DownloadTitle(tmd, path);
        }

        /// <summary>
        /// Download title to path
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        public async Task DownloadTitle(dynamic tmd, string outDir)
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
        public async Task DownloadContent(dynamic tmd, string outDir, int i, IProgress<DownloadContentProgress> progress = null)
        {
            NUSWebClient client = GetNewNUSWebClient();
            Logger.Info($"Downloading {i + 1}/{tmd.Header.ContentCount} {Utility.GetSizeString((long)tmd.Contents[i].Size)}");
            string titleId = tmd.Header.TitleId.ToString("X16");
            string url = nusBaseUrl + titleId + "/" + tmd.Contents[i].ContentId.ToString("X8");
            string dest = Path.Combine(outDir, tmd.Contents[i].ContentId.ToString("X8"));

            if (File.Exists(dest))
            {
                Logger.Info($"'{dest}' already exists, verifying file");
                if (new FileInfo(dest).Length == (long)tmd.Contents[i].Size)
                {
                    Logger.Info("Filesize matches, skipping download.");
                    progress.Report(new DownloadContentProgress
                    {
                        ContentIndex = i,
                        BytesReceived = 0,
                        TotalBytesToReceive = (long)tmd.Contents[i].Size,
                        Status = DownloadContentProgressStatus.AlreadyDownloaded,
                    });
                    return;
                }
                else
                {
                    Logger.Info("Filesize doesn't match, redownloading file.");
                }
            }

            if (progress != null)
            {
                client.DownloadProgressChanged += (sender, e) =>
                {
                    progress.Report(new DownloadContentProgress
                    {
                        ContentIndex = i,
                        BytesReceived = e.BytesReceived,
                        TotalBytesToReceive = (long)tmd.Contents[i].Size,
                        Status = DownloadContentProgressStatus.Downloading,
                    });
                };
            }

            await client.DownloadFileTaskAsync(url, dest);
        }

        /// <summary>
        /// Download title to path, but now in parallel
        /// </summary>
        /// <param name="titleId"></param>
        public async Task DownloadTitleParallel(string titleId, string path, IProgress<DownloadContentProgress> progress = null)
        {
            Logger.Info($"Downloading TMD for TitleID {titleId}");
            dynamic tmd = await DownloadTMD(titleId);
            await DownloadTitleParallel(tmd, path, progress);
        }

        /// <summary>
        /// Download title to path, but now in parallel
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="outDir"></param>
        /// <returns></returns>
        public async Task DownloadTitleParallel(dynamic tmd, string outDir, IProgress<DownloadContentProgress> progress = null)
        {
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < tmd.Header.ContentCount; i++)
            {
                Task downloadContentTask = DownloadContent(tmd, outDir, i, progress);
                tasks.Add(downloadContentTask);

                if (tasks.Count == 4) // 4 = max concurrent downloads
                {
                    var completed = await Task.WhenAny(tasks);
                    tasks.Remove(completed);
                }
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// This is either genius or cheating
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private dynamic LoadTMD(byte[] data)
        {
            var method = TMDType.GetMethod("Load");

            // The load method requires the first parameter to be a `ref byte[] data`.
            // The load method MIGHT have more parameters (which are REQUIRED to be optional), however
            // even if they are optional we still need to pass them (as Type.Missing)
            //
            // TL:DR This code below makes sure that we call TMD.Load() with the correct parameters

            int argc = method.GetParameters().Length;
            object[] argv = new object[argc];
            argv[0] = data;

            for (int i = 1; i < argc; i++)
                argv[i] = Type.Missing;

            return method.Invoke(null, argv);
        }
    }
}
