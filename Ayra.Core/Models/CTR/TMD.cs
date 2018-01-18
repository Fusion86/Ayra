using Ayra.Core.Enums;
using Ayra.Core.Extensions;
using Ayra.Core.Structs.CTR;
using System;

namespace Ayra.Core.Models.CTR
{
    public class TMD_SignatureData
    {
        public NSignatureType Type;
        public byte[] Data;

        public static TMD_SignatureData Load(ref byte[] data)
        {
            TMD_SignatureData signature = new TMD_SignatureData();

            byte[] typeData = new byte[4];
            Buffer.BlockCopy(data, 0, typeData, 0, typeData.Length);

            NSignatureType signatureType = NSignatureType.GetByIdentifier(typeData);
            signature.Type = signatureType;

            byte[] signatureData = new byte[signatureType.SignatureSize];
            Buffer.BlockCopy(data, 0, signatureData, 0, signatureData.Length);
            signature.Data = signatureData;

            return signature;
        }
    }

    public class TMD
    {
        public TMD_SignatureData Signature;
        public _TMD_Header Header;
        public _TMD_ContentInfoRecord[] ContentInfos; // Count = Header.NumContents

        public static TMD Load(ref byte[] data)
        {
            TMD tmd = new TMD();
            tmd.Signature = TMD_SignatureData.Load(ref data);

            int offset = 4 + tmd.Signature.Type.SignatureSize + tmd.Signature.Type.PaddingSize;
            byte[] headerData = new byte[0xC4]; // 0xC4 = sizeof(_TMD_Header)
            Buffer.BlockCopy(data, offset, headerData, 0, headerData.Length);
            tmd.Header = headerData.ToStruct<_TMD_Header>();

            return tmd;
        }
    }
}
