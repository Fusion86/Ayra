using Ayra.Core.Converters;
using Ayra.Core.Enums;
using Newtonsoft.Json;

namespace Ayra.TitleKeyDatabase
{
    public class TitleKeyDatabaseEntryBase
    {
        [JsonProperty("titleID")]
        public string Id { get; set; }

        [JsonProperty("titleKey")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region"), JsonConverter(typeof(NSoftwareRegionConverter))]
        public NSoftwareRegion Region { get; set; }
    }
}
