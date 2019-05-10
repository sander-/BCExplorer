using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Services.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Services
{
    public interface IBlockService
    {
        Task<Block> GetBlock(string id);
        Task<Block> GetLastBlock();
    }

    public class BlockService : IBlockService
    {

        public IBlockProvider BlockProvider { get; set; }
        public IBlockRepository BlockRepository { get; set; }

        public BlockService(IBlockProvider blockProvider,
            IBlockRepository blockRepository)
        {
            BlockProvider = blockProvider;
            BlockRepository = blockRepository;
        }

        public async Task<Block> GetBlock(string id)
        {
            Block block;

            try
            {
                block = BlockRepository.GetById(id);

                if (block == null)
                {
                    block = await BlockProvider.GetBlock(id);

                    if (block != null)
                    {
                        BlockRepository.AddBlock(id, block);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return block;
        }

        public async Task<Block> GetLastBlock()
        {
            var blockhash = await BlockProvider.GetBestBlockHash();
            var block = await GetBlock(blockhash) ?? new Block { Hash = blockhash };
            return block;
        }
    }
}
