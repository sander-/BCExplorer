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
        Task<Address> GetAddress(string id, int page, int itemsOnPage);
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
                var addressTransactions = _transactionService.GetTransactionsForAddress(id);

                var address = new Address()
                {
                    Id = id,
                    Balance = addressFromDb.Balance,
                    LastModifiedBlockHeight = addressFromDb.LastModifiedBlockHeight,
                    Transactions = addressTransactions,
                    TotalTransactions = addressTransactions.Count,
                };
                return address;
            }
        }

        public async Task<Address> GetAddress(string id, int page, int itemsOnPage)
        {
            using (var context = new Model.BCExplorerContext())
            {
                var addressFromDb = await context.Addresses.FindAsync(id);
                if (addressFromDb == null)
                    return null;

                List<Transaction> transactions = new List<Transaction>();
                var addressTransactions = _transactionService.GetTransactionsForAddress(id, page, itemsOnPage);
                var transactionCount =  _transactionService.GetTransactionCountForAddress(id);
                var totalSent = _transactionService.GetTotalSent(id);
                var totalReceived= _transactionService.GetTotalReceived(id);

                var address = new Address()
                {
                    Id = id,
                    Balance = addressFromDb.Balance,
                    LastModifiedBlockHeight = addressFromDb.LastModifiedBlockHeight,
                    Transactions = addressTransactions,
                    TotalTransactions = transactionCount,
                    TotalSent = totalSent,
                    TotalReceived = totalReceived
                };
                return address;
            }
        }
    }
}
