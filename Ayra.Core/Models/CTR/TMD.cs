using Ayra.Core.Extensions;
using Ayra.Core.Structs.CTR;
using System;

namespace Ayra.Core.Models.CTR
{
    public class TMD_SignatureData
    {
        //public NSignatureType Type;
        public byte[] Data;
    }

    public class TMD
    {
        public TMD_SignatureData Signature;
        public _TMD_Header Header;
        //public _TMD_ContentRecord[] Contents; // Count = Header.NumContents

        public static TMD Load(ref byte[] data)
        {
            TMD tmd = new TMD();

            return tmd;
        }
    }
}
