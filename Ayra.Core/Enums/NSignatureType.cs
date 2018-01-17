using Ayra.Core.Extensions;
using System.Linq;

namespace Ayra.Core.Enums
{
    public class NSignatureType : Enumeration
    {
        public readonly int SignatureSize;
        public readonly int PaddingSize;

        public static readonly NSignatureType RSA_4096_SHA1 = new NSignatureType(new byte[] { 0x00, 0x01, 0x00, 0x00 }, "RSA_4096 SHA1", 0x200, 0x3C);
        public static readonly NSignatureType RSA_2048_SHA1 = new NSignatureType(new byte[] { 0x00, 0x01, 0x00, 0x01 }, "RSA_2048 SHA1", 0x100, 0x3C);
        public static readonly NSignatureType ECDSA_SHA1 = new NSignatureType(new byte[] { 0x00, 0x01, 0x00, 0x02 }, "ECDSA with SHA1", 0x3C, 0x40); // I assumed that Elliptic Curve == ECDSA
        public static readonly NSignatureType RSA_4096_SHA256 = new NSignatureType(new byte[] { 0x00, 0x01, 0x00, 0x03 }, "RSA_4096 SHA256", 0x200, 0x3C);
        public static readonly NSignatureType RSA_2048_SHA256 = new NSignatureType(new byte[] { 0x00, 0x01, 0x00, 0x04 }, "RSA_2048 SHA256", 0x100, 0x3C);
        public static readonly NSignatureType ECDSA_SHA256 = new NSignatureType(new byte[] { 0x00, 0x01, 0x00, 0x05 }, "ECDSA with SHA256", 0x3C, 0x40);

        public NSignatureType() { }
        private NSignatureType(byte[] identifier, string name, int signatureSize = 0, int paddingSize = 0) : base(identifier, name)
        {
            SignatureSize = signatureSize;
            PaddingSize = paddingSize;
        }

        public static NSignatureType GetByIdentifier(string id) => GetByIdentifier(id.Substring(0, 8).ParseHexString());
        public static NSignatureType GetByIdentifier(byte[] id) => GetAll<NSignatureType>().FirstOrDefault(x => x.Identifier.SequenceEqual(id));
    }
}
