using Ayra.Core.Extensions;
using System.Linq;

namespace Ayra.Core.Enums
{
    public class NDevice : Enumeration
    {
        public static readonly NDevice N3DS = new NDevice(new byte[] { 0x00, 0x04 }, "Unknown");
        public static readonly NDevice WiiU = new NDevice(new byte[] { 0x00, 0x05 }, "Nintendo Wii U");

        public static readonly NDevice Unknown = new NDevice(null, "Unknown");

        public NDevice() { }
        private NDevice(byte[] identifier, string name) : base(identifier, name) { }

        public static NDevice GetByTitleId(string id) => GetByIdentifier(id);
        public static NDevice GetByIdentifier(string id) => GetByIdentifier(id.Substring(0, 4).ParseHexString());
        public static NDevice GetByIdentifier(byte[] id) => GetAll<NDevice>().FirstOrDefault(x => x.Identifier.SequenceEqual(id));
    }
}
