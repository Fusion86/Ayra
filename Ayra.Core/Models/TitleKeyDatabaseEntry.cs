using Ayra.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Ayra.Core.Enums;

namespace Ayra.Core.Models
{
    public class TitleKeyDatabaseEntry : Title
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public NSoftwareRegion? Region { get; set; }

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
