using BCExplorer;
using BCExplorer.Network;
using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Network.Response;
using BCExplorer.Network.Rpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BCExplorer.Indexer
{
    class Program
    {
        static readonly Client _client;
        static readonly ITransactionProvider _transactionProvider;
        static ILogger _logger;
        static readonly CancellationTokenSource _cts = new CancellationTokenSource();
        static readonly int _interval = 60;
        static readonly int _suspendInterval = 15;
        const string CRLF = "\r\n";

        static int _currentBlockNumber = 1, _currentBlockHeight = 0;

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                    .AddTransient<Program>();

            var serviceProvider = services.BuildServiceProvider();

            _logger = serviceProvider.GetService<ILogger<Program>>();
        }

        static Program()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var config = new ConfigurationBuilder()
                 .AddJsonFile("network-config.json")
                 .Build();

            var settings = new RpcSettings()
            {
                Url = config["url"],
                User = config["user"],
                Password = config["password"]
            };

            _client = new Client(settings, _logger);
            _transactionProvider = new TransactionProvider(_client);
        }

        static void Main(string[] args)
        {
            _logger.Log(LogLevel.Information, "Indexer Starting...press ENTER to cancel.");

            Task.Run(() => Index(_cts.Token));

            Console.ReadLine();
            _logger.Log(LogLevel.Information, $"Stopping");
            _cts.Cancel();
        }

        static async Task Index(CancellationToken token)
        {
            bool wait = false;
            while (!token.IsCancellationRequested)
            {
                if (wait)
                {
                    _logger.Log(LogLevel.Information, $"Waiting for {_interval} seconds.");
                    Thread.Sleep(TimeSpan.FromSeconds(_interval));
                    wait = false;
                }

                try
                {
                    using (var context = new Model.BCExplorerContext())
                    {
                        using (IDbContextTransaction dbtx = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                        {
                            if (await context.Blocks.AnyAsync())
                            {
                                _currentBlockNumber = await context.Blocks.MaxAsync(p => p.Id);
                                _currentBlockHeight = _currentBlockNumber - 1;
                                _currentBlockHeight++;
                                _currentBlockNumber++;
                            }
                        }

                        var tipHash = await _client.GetBestBlockHashAsync();
                        var tip = await _client.GetBlockAsync(tipHash);
                        if (_currentBlockHeight > tip.Height - 3)
                        {
                            Console.WriteLine($"Block at height {_currentBlockHeight} is not yet mature enough....");
                            wait = true;
                            continue;
                        }

                        string blockHash = await _client.GetBlockHashAsync(_currentBlockHeight);

                        var result = await IndexBlock(context, blockHash);
                        if (!result)
                            return;
                    }
                }

                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, $"Error creating transaction: {e.Message}");
                    _logger.Log(LogLevel.Error, $"Suspending for {_suspendInterval} seconds.");
                    Thread.Sleep(TimeSpan.FromSeconds(_suspendInterval));
                    _logger.Log(LogLevel.Error, $"Retrying...");
                }
            }
        }

        private static async Task<bool> IndexBlock(Model.BCExplorerContext context, string blockHash)
        {
            _logger.LogInformation($"Processing block at height {_currentBlockHeight}: {blockHash}");

            BlockResult blockResult = await _client.GetBlockAsync(blockHash);

            if (blockResult == null)
            {
                _logger.LogWarning($"Warning - could not retrieve block at height {_currentBlockHeight}: {blockHash}");
                return false;
            }

            var block = new Model.Block
            {
                Id = _currentBlockNumber,
                Height = _currentBlockHeight,
                BlockHash = blockHash,
                BlockData = blockResult.OriginalJson
            };

            context.Blocks.Add(block);

            if (_currentBlockHeight == 0)
            {
                // for the transaction in the genesis block, we can't pull transaction data, so this is all we know
                var genesisBlockTransaction = new Model.Transaction { Block = block, Id = blockResult.Transactions[0] };
                context.Transactions.Add(genesisBlockTransaction);
                _currentBlockHeight++;
                _currentBlockNumber++;
                context.SaveChanges();
                return true;
            }

            List<BCExplorer.Network.Models.Transaction> blockTransactions = new List<BCExplorer.Network.Models.Transaction>();
            foreach (var txid in blockResult.Transactions)
            {
                var transaction = await _transactionProvider.GetTransaction(txid);
                transaction.Block = new BCExplorer.Network.Models.Block { Height = _currentBlockHeight };
                blockTransactions.Add(transaction);
            }

            foreach (var blockTx in blockTransactions)
            {
                ProcessTransaction(blockTx, context);
            }

            context.SaveChanges();
            _currentBlockHeight++;
            _currentBlockNumber++;
            return true;
        }

        static void ProcessTransaction(Network.Models.Transaction blockTx, Model.BCExplorerContext context)
        {
            switch (blockTx.TransactionType)
            {
                case TransactionType.PoW_Reward_Coinbase:
                    ProcessPoWReward(blockTx, context);
                    break;
                case TransactionType.PoS_Reward:
                    ProcessStakingReward(blockTx, context);
                    break;
                case TransactionType.Money:
                    ProcessMoneyTransfer(blockTx, context);
                    break;
                default:
                    throw new IndexOutOfRangeException("Unsupported TransactionType.");
            }
        }

        static void ProcessPoWReward(Network.Models.Transaction tx, Model.BCExplorerContext context)
        {
            var vout = tx.TransactionsOut[0];
            var address = vout.Address;
            var amount = vout.Value;
            Model.Address existing = context.Addresses.Find(address);
            if (existing == null)
            {
                var newAddress = new Model.Address
                {
                    Id = address,
                    Balance = amount,
                    LastModifiedBlockHeight = (int)tx.Block.Height,
                    TxIdBlob = tx.TransactionId + CRLF
                };
                InsertAddressIfShouldBeIndexed(newAddress, context);
                return;
            }
            existing.Balance += amount;
            existing.LastModifiedBlockHeight = (int)tx.Block.Height;
            UpdateTxIdBlog(existing, tx.TransactionId);
        }

        static void ProcessStakingReward(Network.Models.Transaction tx, Model.BCExplorerContext context)
        {
            var vin = tx.TransactionsIn[0];
            var inAddress = vin.PrevVOutFetchedAddress;
            var oldBalance = vin.PrevVOutFetchedValue;
            
            var outValue1 = tx.TransactionsOut[1].Value;
            var outValue2 = tx.TransactionsOut[2].Value;
            var change = outValue1 + outValue2 - oldBalance;

            // we can assume that only pre-existing addresses get a staking reward
            Model.Address existing = context.Addresses.Find(inAddress);
            if(existing == null)
            {
                _logger.LogError($"{vin.PrevVOutFetchedAddress} could not be found.");
            }
            
            existing.Balance += change;
            existing.LastModifiedBlockHeight = (int)tx.Block.Height;
            UpdateTxIdBlog(existing, tx.TransactionId);
        }

        private static void ProcessMoneyTransfer(Network.Models.Transaction tx, Model.BCExplorerContext context)
        {
            List<Network.Models.Transaction.TransactionIn> vins = tx.TransactionsIn;
            List<string> inAdresses = new List<string>();
            foreach (var vin in vins)
            {
                Model.Address existing = context.Addresses.Find(vin.PrevVOutFetchedAddress);

                if (existing == null) 
                {
                    _logger.LogWarning($"{vin.PrevVOutFetchedAddress} could not be found.");                    
                    continue;
                }

                inAdresses.Add(existing.Id);
                existing.Balance -= vin.PrevVOutFetchedValue;
                existing.LastModifiedBlockHeight = (int)tx.Block.Height;
                UpdateTxIdBlog(existing, tx.TransactionId);
            }
            IList<Network.Models.Transaction.TransactionOut> vouts = tx.TransactionsOut;
            foreach (var vout in vouts)
            {
                string outAdress = vout.Address;
                if (outAdress.StartsWith("OP_RETURN", StringComparison.OrdinalIgnoreCase))
                    outAdress = "OP_RETURN";

                Model.Address existing = context.Addresses.Find(outAdress);
                if (existing != null)
                {
                    existing.Balance += vout.Value;
                    if (!inAdresses.Contains(existing.Id))
                    {
                        UpdateTxIdBlog(existing, tx.TransactionId);
                        existing.LastModifiedBlockHeight = (int)tx.Block.Height;
                    }
                }
                else
                {
                    var newAddress = new Model.Address
                    {
                        Id = vout.Address,
                        Balance = vout.Value,
                        LastModifiedBlockHeight = (int)tx.Block.Height,
                        TxIdBlob = tx.TransactionId + CRLF
                    };
                    InsertAddressIfShouldBeIndexed(newAddress, context);
                }
            }
        }

        private static void UpdateTxIdBlog(Model.Address existing, string transactionId)
        {
            // the last txid is first in the blob
            var oldTxIds = existing.TxIdBlob.Split(CRLF, StringSplitOptions.RemoveEmptyEntries).ToList();
            // append at pos 0
            oldTxIds.Insert(0, transactionId);
            var max250txids = oldTxIds.Take(250).ToArray();
            var sb = new StringBuilder();
            foreach (var txid in max250txids)
                sb.Append(txid + CRLF);
            existing.TxIdBlob = sb.ToString();
        }

        static void InsertAddressIfShouldBeIndexed(Model.Address address, Model.BCExplorerContext context)
        {
            if (address.Id.StartsWith("OP_RETURN", StringComparison.OrdinalIgnoreCase))
                address.Id = "OP_RETURN";
            if (address.Id != TransactionProvider.NON_STANDARD)
                context.Addresses.Add(address);
        }
    }
}
