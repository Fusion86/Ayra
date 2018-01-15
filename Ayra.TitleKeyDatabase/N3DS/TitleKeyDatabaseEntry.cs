using Ayra.Core.Converters;
using Ayra.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

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
