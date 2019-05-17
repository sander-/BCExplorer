using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BCExplorer.Network.Response;

namespace BCExplorer.Services
{
    public interface IBlockService
    {
        Task<Block> GetBlock(string id);
        Task<Block> GetBlockWithTransactions(string id);
        Task<Block> GetLastBlock();
        Task<List<Block>> GetLatestBlocks(int count);
    }

    public class BlockService : IBlockService
    {
        public IBlockProvider BlockProvider { get; set; }
        public IBlockRepository BlockRepository { get; set; }

        readonly ITransactionService _transactionService;

        public BlockService(IBlockProvider blockProvider,
            IBlockRepository blockRepository,
            ITransactionService transactionService
            )
        {
            BlockProvider = blockProvider;
            BlockRepository = blockRepository;
            _transactionService = transactionService;
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

        public async Task<Block> GetBlockWithTransactions(string id)
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
                        foreach (var txid in block.TransactionIds)
                        {
                            var transaction = await _transactionService.GetTransaction(txid);
                            block.Transactions.Add(transaction);
                        }
                        BlockRepository.AddBlockWithTransactions(id, block);
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

        public async Task<List<Block>> GetLatestBlocks(int count)
        {
            List<Block> latestBlocks = new List<Block>();
            using (Model.BCExplorerContext context = new Model.BCExplorerContext())
            {
                var blocks = await context.Blocks.OrderByDescending(p => p.Id).Take(count).ToListAsync();
                foreach (var block in blocks)
                {
                    var blockData = JsonConvert.DeserializeObject<RpcResult<BlockResult>>(block.BlockData);
                    var b = new Block
                    {
                        Bits = blockData.Result.Bits,
                        Chainwork = blockData.Result.Chainwork,
                        Confirmations = blockData.Result.Confirmations,
                        Difficulty = blockData.Result.Difficulty,
                        Hash = blockData.Result.Hash,
                        Height = blockData.Result.Height,
                        MerkleRoot = blockData.Result.Merkleroot,
                        NextBlock = blockData.Result.Nextblockhash,
                        Nonce = blockData.Result.Nonce,
                        PreviousBlock = blockData.Result.Previousblockhash,
                        Size = blockData.Result.Size,
                        Time = blockData.Result.GetTime(),
                        Age = blockData.Result.GetAge(),
                        TotalTransactions = blockData.Result.Transactions.Count,
                        Version = (uint)blockData.Result.Version
                    };

                    latestBlocks.Add(b);
                }
            }
            return latestBlocks;
        }
    }
}
