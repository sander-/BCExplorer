using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Network.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Network
{
    public class BlockProvider : IBlockProvider
    {
        readonly Rpc.IClient _client;

        public BlockProvider(IOptions<Rpc.RpcSettings> options, ILogger logger)
        {
            _client = new Rpc.Client(options.Value, logger);
        }

        public async Task<string> GetBestBlockHash()
        {
            return await _client.GetBestBlockHashAsync();
        }

        public async Task<Block> GetBlock(string id)
        {
            string input = id?.Trim();
            if (input == null || input.Length == 0 || input.Length > 64)
                return null;

            string blockHash = string.Empty;

            int.TryParse(id, out int blockNumber);
            if (blockNumber > 0)
            {
                blockHash = await _client.GetBlockHashAsync(blockNumber);
                if (blockHash == null) return null;
            }
            else
            {
                blockHash = id;
            }

            BlockResult blockResult = await _client.GetBlockAsync(blockHash);
            if (blockResult == null) return null;

            if (blockResult.Transactions == null)
                blockResult.Transactions = new string[0];

            var block = new Block
            {
                Hash = blockResult.Hash,
                Height = blockResult.Height,
                Time = blockResult.GetTime(),
                Difficulty = blockResult.Difficulty,
                MerkleRoot = blockResult.Merkleroot,
                Nonce = blockResult.Nonce,
                PreviousBlock = blockResult.Previousblockhash,
                NextBlock = blockResult.Nextblockhash,
                Confirmations = blockResult.Confirmations,
                TotalTransactions = blockResult.Transactions.Count,
                Transactions = new List<Transaction>(blockResult.Transactions.Count),
                Chainwork = blockResult.Chainwork,
                Bits = blockResult.Bits,
                Size = (int)blockResult.Size,
                //StrippedSize = (int)blockResult.StrippedSize,
                //VersionHex = blockResult.VersionHex,
                //Version = blockResult.Version,
                //Weight = blockResult.Weight

            };
            return block;
        }
    }
}
