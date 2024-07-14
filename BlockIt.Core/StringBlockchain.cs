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
            _blocks = [new Block(0, [], Encoding.UTF8.GetBytes("Genesis Block"))];
        }

        public Block CreateBlock(long timestamp, string data)
        {
            var result = new Block(timestamp, _blocks.Last().BlockHash, Encoding.UTF8.GetBytes(data));
            return result;
        }

        public Block CreateBlock(long timestamp, byte[] data)
        {
            var result = new Block(timestamp, _blocks.Last().BlockHash, data);
            return result;
        }

        public bool HasBlock(Block block)
        {
            //var result = _blocks.Contains(block);
            var result = _blocks.Any(x => x.ContentEqual(block));
            Console.WriteLine($"Check has block: {result}");
            return result;
        }

        public void Add(Block block)
        {
            _blocks.Add(block);
        }

        public void Sort() 
        {
            var sortedBlocks = new SortedList<long, Block>(_blocks.ToDictionary(x => x.Timestamp));
            var newBlocks = new List<Block>();
            foreach (var sortedblock in sortedBlocks)
            {
                newBlocks.Add(CreateBlock(sortedblock.Value.Timestamp, sortedblock.Value.Data));
            }
            _blocks = newBlocks;
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
