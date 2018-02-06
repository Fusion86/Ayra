using Ayra.Core.Extensions;
using System.Linq;

namespace Ayra.Core.Enums
{
    public class NSoftwarePlatform : Enumeration
    {
        public static readonly NSoftwarePlatform N3DS = new NSoftwarePlatform(new byte[] { 0x00, 0x04 }, "Nintendo 3DS");
        public static readonly NSoftwarePlatform WiiU = new NSoftwarePlatform(new byte[] { 0x00, 0x05 }, "Nintendo Wii U");

        public static readonly NSoftwarePlatform Unknown = new NSoftwarePlatform(null, "Unknown");

        public NSoftwarePlatform() { }
        private NSoftwarePlatform(byte[] identifier, string name) : base(identifier, name) { }

        public static NSoftwarePlatform GetByTitleId(string id) => GetByIdentifier(id);
        public static NSoftwarePlatform GetByIdentifier(string id) => GetByIdentifier(id.Substring(0, 4).ParseHexString());
        public static NSoftwarePlatform GetByIdentifier(byte[] id) => GetAll<NSoftwarePlatform>().FirstOrDefault(x => x.Identifier.SequenceEqual(id));
    }
}
