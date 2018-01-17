using Ayra.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ayra.Core.Models
{
    public class NSignatureType
    {
        public byte[] Identifier { get; }
        public string Name { get; }
        public int SignatureSize;
        public int PaddingSize;

        public NSignatureType(string identifier, string name, int signatureSize, int paddingSize)
        {
            Identifier = identifier.ParseHexString();
            Name = name;
            SignatureSize = signatureSize;
            PaddingSize = paddingSize;
        }
    }

    public static class NSignatureTypes
    {
        public static NSignatureType RSA_4096_SHA256 = new NSignatureType("010003", "RSA_4096 SHA256", 0x200, 0x3C);
        public static NSignatureType RSA_2048_SHA256 = new NSignatureType("010004", "RSA_2048 SHA256", 0x100, 0x3C);
        public static NSignatureType ECDSA_SHA256 = new NSignatureType("010005", "ECDSA with SHA256", 0x100, 0x3C);
        public static NSignatureType Unknown = new NSignatureType(null, "Unknown", 0, 0);

        public static NSignatureType GetByTitleId(string id) => GetByTitleId(id.ParseHexString());

        public static NSignatureType GetByTitleId(byte[] id)
        {
            IEnumerable<NSignatureType> types = typeof(NSignatureType).GetFields().Select(x => (NSignatureType)x.GetValue(null)); // TODO: Maybe make compile time const, if possible

            NSignatureType type = types.First(x => x.Identifier == id);
            return type ?? Unknown;
        }
    }
}
