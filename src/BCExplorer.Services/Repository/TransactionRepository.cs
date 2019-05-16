using BCExplorer.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Services.Repository
{
    public interface ITransactionRepository
    {
        Transaction GetById(string id);
        void AddTransaction(string id, Transaction transaction);
    }
    public class TransactionRepository : ITransactionRepository
    {
        static readonly Dictionary<string, Transaction> _transactions = new Dictionary<string, Transaction>();

        public Transaction GetById(string id)
        {
            return _transactions.ContainsKey(id) ? _transactions[id] : null;
        }

        public void AddTransaction(string id, Transaction transaction)
        {
            if (!_transactions.ContainsKey(id)) _transactions.Add(id, transaction);
        }
    }
}
