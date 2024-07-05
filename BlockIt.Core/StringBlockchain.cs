using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.Core
{
    public class StringBlockchain
    {
        private List<Block> _blocks;

        public int Count => _blocks.Count;

        public StringBlockchain() 
        {
            _blocks = [new Block([], Encoding.UTF8.GetBytes("Genesis Block"))];
        }

        public void Add(string data)
        {
            _blocks.Add(new Block(_blocks.Last().BlockHash, Encoding.UTF8.GetBytes(data)));
        }

        public string PrintBlock(int index)
        {
            var block = _blocks[index];
            StringBuilder result = new StringBuilder();

            result.Append($"Block ID: {index}{Environment.NewLine}");
            result.Append($"Timestamp : {block.Timestamp}{Environment.NewLine}");
            result.Append($"Hash of previous block: 0x{Convert.ToHexString(block.PreviousBlockHash)}{Environment.NewLine}");
            result.Append($"Hash of the block: 0x{Convert.ToHexString(block.BlockHash)}{Environment.NewLine}");
            result.Append($"Data: {Encoding.UTF8.GetString(block.Data)}{Environment.NewLine}");
            result.Append($"{Environment.NewLine}");
            return result.ToString();
        }
    }
}
