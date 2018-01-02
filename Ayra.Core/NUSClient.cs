using Ayra.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ayra.Core
{
    public static class NUSClient
    {
        /// <summary>
        /// Download TMD
        /// </summary>
        /// <param name="title">Title to download the metadata for</param>
        /// <returns>TMD byte array</returns>
        public static async Task<TitleMetaData> DownloadTitleMetadata(Title title)
        {
            string url = "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/" + title.Id + "/tmd";
            byte[] data = await new WebClient().DownloadDataTaskAsync(new Uri(url));
            return TitleMetaData.Load(data);
        }
    }
}
