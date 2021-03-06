﻿using BCExplorer.Network.Models;
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
        Block GetByIdWithTransactions(string id);
        void AddBlockWithTransactions(string id, Block block);
    }

    public class BlockRepository : IBlockRepository
    {
        static readonly Dictionary<string, Block> _blocks = new Dictionary<string, Block>();
        static readonly Dictionary<string, Block> _blocksWithTransactions = new Dictionary<string, Block>();

        public Block GetById(string id)
        {
            return _blocks.ContainsKey(id) ? _blocks[id] : null;
        }
        public void AddBlock(string id, Block block)
        {
            if (!_blocks.ContainsKey(id)) _blocks.Add(id, block);
        }
        public Block GetByIdWithTransactions(string id)
        {
            return _blocksWithTransactions.ContainsKey(id) ? _blocksWithTransactions[id] : null;
        }
        public void AddBlockWithTransactions(string id, Block block)
        {
            if (!_blocksWithTransactions.ContainsKey(id)) _blocksWithTransactions.Add(id, block);
        }
    }
}
