using Ayra.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Ayra.Core.Converters
{
    public class NSoftwareRegionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // writer.WriteValue(((bool)value) ? 1 : 0);
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            NSoftwareRegion region = new NSoftwareRegion();
            string str = reader.Value.ToString();

            for (int i = 0; i < str.Length / 3; i++)
                region |= (NSoftwareRegion)Enum.Parse(typeof(NSoftwareRegion), str.Substring(i * 3, 3));

            return region;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NSoftwareRegion);
        }
    }
}
