using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Services.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Services
{
    public interface ITransactionService
    {
        Task<Transaction> GetTransaction(string transactionId);
        IList<AddressTransaction> GetTransactionsForAddress(string addressId);
    }

    public class TransactionService : ITransactionService
    {
        public ITransactionProvider TransactionProvider { get; set; }
        public ITransactionRepository TransactionRepository { get; set; }
        public IBlockProvider BlockProvider { get; set; }

        public TransactionService(ITransactionProvider transactionProvider,
            ITransactionRepository transactionRepository,
            IBlockProvider blockProvider
            )
        {
            TransactionProvider = transactionProvider;
            TransactionRepository = transactionRepository;
            BlockProvider = blockProvider;
        }

        public async Task<Transaction> GetTransaction(string transactionId)
        {
            Transaction transaction;

            try
            {
                transaction = TransactionRepository.GetById(transactionId);

                if (transaction == null)
                {
                    transaction = await GetTransactionFromDb(transactionId);

                    if (transaction != null)
                        TransactionRepository.AddTransaction(transactionId, transaction);
                }

                if (transaction == null)
                {
                    transaction = await TransactionProvider.GetTransaction(transactionId);
                    transaction.Block = await BlockProvider.GetBlock(transaction.Blockhash);

                    if (transaction != null)
                    {
                        TransactionRepository.AddTransaction(transactionId, transaction);
                        await StoreTransactionToDb(transaction);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return transaction;
        }

        private async Task<Transaction> GetTransactionFromDb(string transactionId)
        {
            using (var context = new Model.BCExplorerContext())
            {
                var transactionFromDb = await context.Transactions.FindAsync(transactionId);
                if (transactionFromDb == null)
                    return null;

                var transaction = JsonConvert.DeserializeObject<Transaction>(transactionFromDb.TransactionData);
                return transaction;
            }
        }

        private async Task StoreTransactionToDb(Transaction transaction)
        {
            using (var context = new Model.BCExplorerContext())
            {
                var transactionFromDb = await context.Transactions.FindAsync(transaction.TransactionId);
                var blockFromDb = context.Blocks.FirstOrDefault(p => p.Height == transaction.Block.Height);

                if (transactionFromDb == null && blockFromDb != null)
                {
                    transactionFromDb = new Model.Transaction
                    {
                        TransactionData = JsonConvert.SerializeObject(transaction),
                        Block = blockFromDb,
                        Id = transaction.TransactionId
                    };
                    context.Transactions.Add(transactionFromDb);
                    context.SaveChanges();
                }
            }
        }
        public IList<AddressTransaction> GetTransactionsForAddress(string addressId)
        {
            using (var context = new Model.BCExplorerContext())
            {
                var addressTransactions = new List<AddressTransaction>();
                var transactionsFromDb =
                    context.AddressTransactions.Where(p => p.Address.Id == addressId).OrderBy(p => p.TimeStamp).ThenByDescending(p => p.Balance);

                foreach (var tx in transactionsFromDb)
                {
                    var at = new AddressTransaction
                    {
                        AddressId = addressId,
                        Amount = tx.Amount,
                        Balance = tx.Balance,
                        TransactionId = tx.TransactionId,
                        Time = tx.TimeStamp
                    };
                    addressTransactions.Add(at);
                }

                return addressTransactions;
            }
        }
    }
}
