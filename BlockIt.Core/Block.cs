using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BlockIt.Core.Helper;
using System.Diagnostics.CodeAnalysis;

namespace BlockIt.Core
{
    public class Block
    {
        public long Timestamp { get; private set; }
        public byte[] PreviousBlockHash { get; private set; }
        public byte[] Data { get; private set; }
        public byte[] BlockHash { get; private set; }
        public byte[] ContentHash { get; private set; }

        public Block(long timestamp, byte[] previousBlockHash, byte[] data) 
        {
            Timestamp = timestamp;
            PreviousBlockHash = previousBlockHash;
            Data = data;

            var blockBytes = ByteHelper.CombineByteArrays(BitConverter.GetBytes(Timestamp), previousBlockHash, data);
            BlockHash = SHA256.HashData(blockBytes);

            var contentBytes = ByteHelper.CombineByteArrays(BitConverter.GetBytes(Timestamp), data);
            ContentHash = SHA256.HashData(contentBytes);
        }

        public bool ContentEqual(Block? other)
        {
            if (other == null)
                return false;
            var otherContentHashString = $"0x{Convert.ToHexString(other.ContentHash)}";
            var contentHashString = $"0x{Convert.ToHexString(ContentHash)}";
            return otherContentHashString.Equals(contentHashString);
        }

        public bool BlockEqual(Block? other) 
        {
            if (other == null)
                return false;
            var otherBlockHashString = $"0x{Convert.ToHexString(other.BlockHash)}";
            var blockHashString = $"0x{Convert.ToHexString(BlockHash)}";
            return otherBlockHashString.Equals(blockHashString);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Data);
        }
    }
}
