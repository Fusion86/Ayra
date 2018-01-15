using Ayra.Core.Converters;
using Ayra.Core.Enums;
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

        public async Task<byte[]> DownloadTicket(string url)
        {
            if (!HasTicket) return null;
            if (url.EndsWith("/")) url.TrimEnd('/');
            return await new WebClient().DownloadDataTaskAsync(new Uri(url + "/ticket/" + Id + ".tik"));
        }
    }
}
