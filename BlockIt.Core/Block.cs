using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BlockIt.Core
{
    public class Block
    {
        public long Timestamp { get; private set; }
        public byte[] PreviousBlockHash { get; private set; }
        public byte[] Data { get; private set; }
        public byte[] BlockHash { get; private set; }

        public Block(byte[] previousBlockHash, byte[] data) 
        {
            Timestamp = DateTime.UtcNow.Ticks;
            PreviousBlockHash = previousBlockHash;
            Data = data;

            var bytesToHash = CombineByteArrays(BitConverter.GetBytes(Timestamp), previousBlockHash, data);
            BlockHash = SHA256.HashData(bytesToHash);
        }

        private byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] destinationArray = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, destinationArray, offset, array.Length);
                offset += array.Length;
            }
            return destinationArray;
        }
    }
}
