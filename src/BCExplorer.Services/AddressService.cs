using BCExplorer.Network.Models;
using System;
using System.Collections.Generic;
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

        public async Task<Address> GetAddress(string id)
        {
            using (var context = new Model.BCExplorerContext())
            {
                var addressFromDb = await context.Addresses.FindAsync(id);
                if (addressFromDb == null)
                    return null;

                List<Transaction> transactions = new List<Transaction>();
                string[] txids = addressFromDb.TxIdBlob.Split(CRLF, StringSplitOptions.RemoveEmptyEntries);

                foreach (var txid in txids)
                {
                    var transaction = new Transaction()
                    {
                        TransactionId = txid
                    };
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
