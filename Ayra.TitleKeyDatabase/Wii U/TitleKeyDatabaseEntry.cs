using Ayra.Core.Converters;
using Ayra.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Ayra.TitleKeyDatabase.Wii_U
{
    /// <summary>
    /// Only useful when using a website based on TomEke/WiiU-Title-Key
    /// </summary>
    public class TitleKeyDatabaseEntry
    {
        [JsonProperty("titleID")]
        public string Id { get; set; }

        [JsonProperty("titleKey")]
        public string Key { get; set; }

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
