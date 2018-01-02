using Newtonsoft.Json;

namespace Ayra.Core.Models
{
    public class Title
    {
        [JsonProperty("titleID")]
        public string Id { get; set; }

        [JsonProperty("titleKey")]
        public string Key { get; set; }

        [JsonIgnore]
        public NSoftwareType Type => NSoftwareTypes.GetByHeader(Id.Substring(0, 8));
    }
}
