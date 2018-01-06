using Ayra.Core.Enums;
using Ayra.Core.Extensions;
using Ayra.Core.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Ayra.Core
{
    public class NUSClient : IDisposable
    {
        private const string WIIU_NUS_URL = "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";
        private const string WIIU_USER_AGENT = "wii libnup/1.0";

        private readonly string nusBaseUrl;
        private readonly NUSWebClient webClient;

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
                    webClient = new NUSWebClient(WIIU_USER_AGENT);
                    break;
            }

            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChangedEventHandler?.Invoke(sender, e);
            Debug.WriteLine($"Download progress: {e.ProgressPercentage}% - {Utility.GetSizeString(e.BytesReceived)}/{Utility.GetSizeString(e.TotalBytesToReceive)}");
        }

        #region IDisposable Members
        private bool isDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                webClient.Dispose();
            }

            isDisposed = true;
        }
        #endregion

        /// <summary>
        /// Download TitleMetaData
        /// </summary>
        /// <param name="titleId">Title to download the metadata for</param>
        /// <returns></returns>
        public async Task<TMD> DownloadTMD(string titleId)
        {
            string url = nusBaseUrl + titleId + "/tmd";
            byte[] data = await webClient.DownloadDataTaskAsync(new Uri(url));
            return TMD.Load(data);
        }

        /// <summary>
        /// Download Common E-Ticket
        /// </summary>
        /// <param name="titleId"></param>
        /// <returns></returns>
        public async Task<object> DownloadCetk(string titleId)
        {
            string url = nusBaseUrl + titleId + "/cetk";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Download title to path
        /// </summary>
        /// <param name="titleId"></param>
        public async void DownloadTitle(string titleId, string path)
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
                Debug.WriteLine($"[DownloadTitle] Downloading {i+1}/{tmd.Header.NumContents} {Utility.GetSizeString((long)tmd.Contents[i].Size)}");
                
                string titleId = tmd.Header.TitleId.ToString("X16");
                string url = nusBaseUrl + titleId + "/" + tmd.Contents[i].ContentId.ToString("X8");
                await webClient.DownloadFileTaskAsync(url, Path.Combine(outDir, tmd.Contents[i].ContentId.ToString("X8")));
            }
        }
    }
}
