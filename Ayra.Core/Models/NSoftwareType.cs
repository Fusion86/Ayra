using System.Collections.Generic;
using System.Linq;

namespace Ayra.Core.Models
{
    public class NSoftwareType
    {
        public string[] Headers { get; }
        public string Name { get; }

        public NSoftwareType(string[] headers, string name)
        {
            Headers = headers;
            Name = name;
        }
    }

    public class NSoftwareTypes
    {
        public static NSoftwareType SystemApplication = new NSoftwareType(new[] { "00050010", "0005001B" }, "System Application");
        public static NSoftwareType eShopApplication = new NSoftwareType(new[] { "00050000" }, "eShop/Application");
        public static NSoftwareType Demo = new NSoftwareType(new[] { "00050002" }, "Demo");
        public static NSoftwareType Update = new NSoftwareType(new[] { "0005000E" }, "Update");
        public static NSoftwareType DLC = new NSoftwareType(new[] { "0005000C" }, "DLC");
        public static NSoftwareType Unknown = new NSoftwareType(null, "Unknown");

        public static NSoftwareType GetByTitleId(string id)
        {
            IEnumerable<NSoftwareType> types = typeof(NSoftwareTypes).GetFields().Select(x => (NSoftwareType)x.GetValue(null)); // TODO: Maybe make compile time const, if possible

            if (id.Length > 8) id = id.Substring(0, 8);
            NSoftwareType type = types.First(x => x.Headers.Contains(id.ToUpper()));

            return type ?? Unknown;
        }
    }
}
