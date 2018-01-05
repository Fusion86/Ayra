using Ayra.Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace Ayra.Core
{
    public static class TitleKeyDatabase
    {
        /// <summary>
        /// Get TitleKeyDatabaseEntry from website (realistically only one website is supported)
        /// </summary>
        /// <param name="url">Website url</param>
        /// <returns></returns>
        public static List<TitleKeyDatabaseEntry> GetAllEntries(string url)
        {
            string json = new WebClient().DownloadString(url + "/json");
            List<TitleKeyDatabaseEntry> keys = JsonConvert.DeserializeObject<List<TitleKeyDatabaseEntry>>(json);
            return keys;
        }
    }
}
