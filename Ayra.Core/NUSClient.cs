using Ayra.Core.Enums;
using Ayra.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ayra.Core
{
    public class NUSClient : IDisposable
    {
        private const string WII_NUS_URL = "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";
        private const string WII_USER_AGENT = "wii libnup/1.0";

        private readonly string nusBaseUrl;
        private readonly NUSWebClient webClient;

        public NUSClient(NDevice device)
        {
            switch (device)
            {
                case NDevice.DSI:
                    throw new NotImplementedException();

                case NDevice.WII:
                    throw new NotImplementedException();

                case NDevice.WII_U:
                    nusBaseUrl = WII_NUS_URL;
                    webClient = new NUSWebClient(WII_USER_AGENT);
                    break;
            }
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
        /// Download TMD
        /// </summary>
        /// <param name="title">Title to download the metadata for</param>
        /// <returns>TMD byte array</returns>
        public async Task<TitleMetaData> DownloadTitleMetadata(Title title)
        {
            string url = nusBaseUrl + title.Id + "/tmd";
            byte[] data = await new WebClient().DownloadDataTaskAsync(new Uri(url));
            return TitleMetaData.Load(data);
        }
    }
}
