using Ayra.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ayra.Core.Enums
{
    /// <summary>
    /// For 3DS and WiiU
    /// </summary>
    public class NSoftwareType : Enumeration
    {
        // TODO: Maybe use bitmasks?
        // e.g Firmware = Normal | CannotExecution | System | RequireBatchUpdate | CanSkipConvertJumpId

        // System Titles
        public static readonly NSoftwareType SystemApplication = new NSoftwareType(new byte[] { 0x00, 0x10 }, "System Application"); // SYSTEM_APPLICATION
        public static readonly NSoftwareType SystemDataArchive = new NSoftwareType(new byte[] { 0x00, 0x1B }, "System Data Archive"); // SYSTEM_CONTENT
        public static readonly NSoftwareType SharedDataArchive = new NSoftwareType(new byte[] { 0x00, 0x9B }, "Shared Data Archives"); // SHARED_CONTENT
        public static readonly NSoftwareType SystemAutoUpdateDataArchive = new NSoftwareType(new byte[] { 0x00, 0xDB }, "System Data Archives"); // AUTO_UPDATE_CONTENT
        public static readonly NSoftwareType Applet = new NSoftwareType(new byte[] { 0x00, 0x30 }, "Applet"); // APPLET
        public static readonly NSoftwareType Module = new NSoftwareType(new byte[] { 0x01, 0x30 }, "Module"); // BASE
        public static readonly NSoftwareType Firmware = new NSoftwareType(new byte[] { 0x01, 0x38 }, "Firmware"); // FIRMWARE

        // Application titles
        public static readonly NSoftwareType Application = new NSoftwareType(new byte[] { 0x00, 0x00 }, "Application");
        public static readonly NSoftwareType DownloadPlayChild = new NSoftwareType(new byte[] { 0x00, 0x01 }, "Download Play Child");
        public static readonly NSoftwareType Demo = new NSoftwareType(new byte[] { 0x00, 0x02 }, "Demo");
        public static readonly NSoftwareType DLC = new NSoftwareType(new byte[] { 0x00, 0x0C }, "Downloadable Content");
        public static readonly NSoftwareType Update = new NSoftwareType(new byte[] { 0x00, 0x0E }, "Update");

        public NSoftwareType() { }
        private NSoftwareType(byte[] identifier, string name) : base(identifier, name) { }

        public static NSoftwareType GetByTitleId(string id) => GetByIdentifier(id.Substring(4, 4).ParseHexString());
        public static NSoftwareType GetByIdentifier(string id) => GetByIdentifier(id.Substring(0, 4).ParseHexString());
        public static NSoftwareType GetByIdentifier(byte[] id) => GetAll<NSoftwareType>().FirstOrDefault(x => x.Identifier.SequenceEqual(id));
    }
}
