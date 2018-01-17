using Ayra.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Ayra.TitleKeyDatabase.Wii_U
{
    public class TitleKeyDatabaseEntry : TitleKeyDatabaseEntryBase
    {
        [JsonProperty("ticket"), JsonConverter(typeof(BoolConverter))]
        public bool HasTicket { get; set; }

        public async Task<byte[]> DownloadTicket()
        {
            if (!HasTicket) return null;
            return await new WebClient().DownloadDataTaskAsync(new Uri(Config.Url + "/ticket/" + TitleId + ".tik"));
        }
    }
}
