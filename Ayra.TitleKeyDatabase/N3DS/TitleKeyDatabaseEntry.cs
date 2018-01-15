using Newtonsoft.Json;

namespace Ayra.TitleKeyDatabase.N3DS
{
    public class TitleKeyDatabaseEntry : TitleKeyDatabaseEntryBase
    {
        [JsonProperty("encTitleKey")]
        public string EncryptedTitleKey { get; set; }

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("size")]
        public ulong? Size { get; set; }
    }
}
