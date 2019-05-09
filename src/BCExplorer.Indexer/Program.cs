using BCExplorer.Model;
using BCExplorer.Network;
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
                    using (var context = new BCExplorerContext())
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
                        if (result != 0)
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

        private static async Task<int> IndexBlock(BCExplorerContext context, string blockHash)
        {
            _logger.LogInformation($"Processing block at height {_currentBlockHeight}: {blockHash}");

            BlockResult blockResult = await _client.GetBlockAsync(blockHash);

            if (blockResult == null)
            {
                _logger.LogWarning($"Warning - could not retrieve block at height {_currentBlockHeight}: {blockHash}");
                return -1;
            }

            var block = new Block
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
                var genesisBlockTransaction = new Transaction { Block = block, Id = blockResult.Transactions[0] };
                context.Transactions.Add(genesisBlockTransaction);
                _currentBlockHeight++;
                _currentBlockNumber++;
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                    throw;
                }

                return 0;
            }

            List<BCExplorer.Network.Models.Transaction> blockTransactions = new List<BCExplorer.Network.Models.Transaction>();
            foreach (var txid in blockResult.Transactions)
            {
                var transaction = await _transactionProvider.GetTransaction(txid);
                transaction.Block = new BCExplorer.Network.Models.Block { Height = _currentBlockHeight };
                blockTransactions.Add(transaction);
            }

            return 0;
            //foreach (var blockTx in blockTransactions)
            //{
            //    var transaction = new Transaction()
            //    {
            //        Id = blockTx.
            //    }
            //}
        }
    }
}
