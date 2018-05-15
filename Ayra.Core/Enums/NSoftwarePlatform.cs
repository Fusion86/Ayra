using Ayra.Core.Extensions;
using System.Linq;

namespace Ayra.Core.Enums
{
    public class NSoftwarePlatform : Enumeration
    {
        public static readonly NSoftwarePlatform CTR = new NSoftwarePlatform(new byte[] { 0x00, 0x04 }, "Nintendo 3DS");
        public static readonly NSoftwarePlatform WUP = new NSoftwarePlatform(new byte[] { 0x00, 0x05 }, "Nintendo Wii U");
        public static readonly NSoftwarePlatform NX = new NSoftwarePlatform(new byte [] { 0x01, 00 }, "Nintendo Switch");

        public NSoftwarePlatform() { }
        private NSoftwarePlatform(byte[] identifier, string name) : base(identifier, name) { }

        public static NSoftwarePlatform GetByTitleId(string id) => GetByIdentifier(id);
        public static NSoftwarePlatform GetByIdentifier(string id) => GetByIdentifier(id.Substring(0, 4).ParseHexString());
        public static NSoftwarePlatform GetByIdentifier(byte[] id) => GetAll<NSoftwarePlatform>().FirstOrDefault(x => x.Identifier.SequenceEqual(id));
    }
}
