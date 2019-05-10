using BCExplorer.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Services.Repository
{
    public interface IBlockRepository
    {
        Block GetById(string id);
        void AddBlock(string id, Block block);
    }

    public class BlockRepository : IBlockRepository
    {
        static Dictionary<string, Block> _blocks = new Dictionary<string, Block>();

        public Block GetById(string id)
        {
            return _blocks.ContainsKey(id) ? _blocks[id] : null;
        }
        public void AddBlock(string id, Block block)
        {
            if (!_blocks.ContainsKey(id)) _blocks.Add(id, block);
        }
    }
}
