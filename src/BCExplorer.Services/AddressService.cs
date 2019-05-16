using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Services
{
    public interface IAddressService
    {
        Task<Address> GetAddress(string id);
    }

    public class AddressService : IAddressService
    {
        readonly char[] CRLF = "\r\n".ToCharArray();

        readonly ITransactionService _transactionService;

        public AddressService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Address> GetAddress(string id)
        {
            using (var context = new Model.BCExplorerContext())
            {
                var addressFromDb = await context.Addresses.FindAsync(id);
                if (addressFromDb == null)
                    return null;

                List<Transaction> transactions = new List<Transaction>();
                string[] txids = addressFromDb.TxIdBlob.Split(CRLF, StringSplitOptions.RemoveEmptyEntries);

                foreach (var txid in txids.Distinct())
                {
                    var transaction = await _transactionService.GetTransaction(txid);                    
                    transactions.Add(transaction);
                }

                var address = new Address()
                {
                    Id = id,
                    Balance = addressFromDb.Balance,
                    LastModifiedBlockHeight = addressFromDb.LastModifiedBlockHeight,
                    TransactionIds = txids,
                    TotalTransactions = txids.Length,
                    Transactions = transactions
                };
                return address;
            }
        }
    }
}
