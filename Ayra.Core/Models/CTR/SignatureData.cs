using Ayra.Core.Enums;
using System;

namespace Ayra.Core.Models.CTR
{
    public class SignatureData
    {
        public NSignatureType Type;
        public byte[] Data;

        public static SignatureData Load(byte[] data)
        {
            SignatureData signature = new SignatureData();

            byte[] typeData = new byte[4];
            Buffer.BlockCopy(data, 0, typeData, 0, typeData.Length);

            NSignatureType signatureType = NSignatureType.GetByIdentifier(typeData);
            signature.Type = signatureType;

            byte[] signatureData = new byte[signatureType.SignatureSize];
            Buffer.BlockCopy(data, 4, signatureData, 0, signatureData.Length);
            signature.Data = signatureData;

            return signature;
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[4 + Type.SignatureSize + Type.PaddingSize];

            Buffer.BlockCopy(Type.Identifier, 0, data, 0, 4);
            Buffer.BlockCopy(Data, 0, data, 4, Data.Length);
            // rest of the file is padding which is 0x00 by default already

            return data;
        }
    }
}
