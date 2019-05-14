using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Services.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Services
{
    public interface ITransactionService
    {
        Task<Transaction> GetTransaction(string transactionId);
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
                    transaction = await TransactionProvider.GetTransaction(transactionId);
                    transaction.Block = await BlockProvider.GetBlock(transaction.Blockhash);

                    if (transaction != null)
                    {
                        TransactionRepository.AddTransaction(transactionId, transaction);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return transaction;
        }
    }
}
