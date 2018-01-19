using Ayra.Core.Enums;
using Ayra.Core.Extensions;
using Ayra.Core.Structs.CTR;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace Ayra.Core.Models.CTR
{
    public class TMD_SignatureData
    {
        public NSignatureType Type;
        public byte[] Data;

        public static TMD_SignatureData Load(byte[] data)
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
        public _TMD_ContentInfoRecord[] ContentInfos; // Count = 64
        public _TMD_ContentChunkRecord[] ContentChunks; // Count = Header.ContentCount

        public static TMD Load(byte[] data, bool verifyHash = true)
        {
            SHA256 sha = null;
            if (verifyHash) sha = SHA256.Create();

            TMD tmd = new TMD();
            tmd.Signature = TMD_SignatureData.Load(data);

            int offset = 4 + tmd.Signature.Type.SignatureSize + tmd.Signature.Type.PaddingSize;
            byte[] headerData = new byte[0xC4]; // 0xC4 = sizeof(_TMD_Header)
            Buffer.BlockCopy(data, offset, headerData, 0, headerData.Length);
            tmd.Header = headerData.ToStruct<_TMD_Header>();

            offset += headerData.Length; // Set baseoffset to right after header (aka the start of the content info records)

            // ContentInfoRecords (all of them) hash verification
            if (verifyHash)
            {
                byte[] contentInfoRecordsData = new byte[0x24 * 64]; // 0x24 = sizeof(_TMD_ContentInfoRecord)
                Buffer.BlockCopy(data, offset, contentInfoRecordsData,
                    0, contentInfoRecordsData.Length);
                byte[] hash = sha.ComputeHash(contentInfoRecordsData);

                if (hash.SequenceEqual(tmd.Header.Hash))
                    Debug.WriteLine("[TMD.Load] Correct ContentInfoRecords hash!");
                else
                    throw new Exception("ContentInfoRecords hash is not valid!");
            }

            tmd.ContentInfos = new _TMD_ContentInfoRecord[64];
            byte[] contentInfoData = new byte[0x24]; // 0x24 = sizeof(_TMD_ContentInfoRecord)
            for (int i = 0; i < 64; i++)
            {
                Buffer.BlockCopy(data, offset + i * contentInfoData.Length,
                    contentInfoData, 0, contentInfoData.Length);
                tmd.ContentInfos[i] = contentInfoData.ToStruct<_TMD_ContentInfoRecord>();
            }

            offset += 0x24 * 64; // Set baseoffset to right after the content info records (aka the start of the content chunks)

            tmd.ContentChunks = new _TMD_ContentChunkRecord[tmd.Header.ContentCount];
            byte[] contentChunkData = new byte[0x30]; // 0x24 = sizeof(_TMD_ContentChunkRecord)
            for (int i = 0; i < tmd.Header.ContentCount; i++)
            {
                Buffer.BlockCopy(data, offset + i * contentChunkData.Length,
                    contentChunkData, 0, contentChunkData.Length);
                tmd.ContentChunks[i] = contentChunkData.ToStruct<_TMD_ContentChunkRecord>();

                // FIXME: Hash verification is not correct
                //// ContentChunk (single instance) hash verification
                //if (verifyHash)
                //{
                //    byte[] contentChunkDataWithoutHash = new byte[0x10]; // 0x30 - 0x20 = total_chunk_size - hash_size
                //    Buffer.BlockCopy(contentChunkData, 0,
                //        contentChunkDataWithoutHash, 0, contentChunkDataWithoutHash.Length);
                //    byte[] hash = sha.ComputeHash(contentChunkData);

                //    if (hash.SequenceEqual(tmd.ContentChunks[i].Hash))
                //        Debug.WriteLine($"[TMD.Load] Correct ContentChunk[{i}] hash!");
                //    else
                //        throw new Exception($"ContentChunk[{i}] hash is not valid!");
                //}
            }

            return tmd;
        }
    }
}
