using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Ayra.Core.Classes
{
    public abstract class NUSClient
    {
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
            return LoadTMD(ref data);
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
            dynamic tmd = await DownloadTMD(titleId);
            await DownloadTitle(tmd, path);
        }

        /// <summary>
        /// Download title to path, but now in parallel
        /// </summary>
        /// <param name="titleId"></param>
        public async Task DownloadTitleParallel(string titleId, string path, int maxConcurrent = 4)
        {
            Debug.WriteLine($"[DownloadTitle] Downloading TMD for TitleID {titleId}");
            dynamic tmd = await DownloadTMD(titleId);
            await DownloadTitleParallel(tmd, path, maxConcurrent);
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

        public abstract Task DownloadTitle(dynamic tmd, string outDir);
        public abstract Task DownloadContent(dynamic tmd, string outDir, int i);

        private dynamic LoadTMD(ref byte[] data)
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
