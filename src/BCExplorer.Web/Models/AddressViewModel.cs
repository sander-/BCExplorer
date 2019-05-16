using BCExplorer.Network.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCExplorer.Web.Models
{
    public class AddressViewModel
    {
        public Address Address { get; set; }
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        public int Max { get; set; }
        public int OffSet { get; set; }

        public decimal TotalReceived { get; set; }
        public decimal TotalSent { get; set; }
        public IList<Transaction> Transactions { get; set; }
        public IList<AddressTransaction> AddressTransactions { get; set; }

        public class AddressTransaction
        {
            public Transaction Transaction { get; set; }
            public Address Address { get; set; }
            public decimal TotalIn { get; set; }
            public decimal TotalOut { get; set; }
            public decimal Balance { get; set; }
        }

        public void CalculateTotals()
        {
            AddressTransactions = new List<AddressTransaction>();

            decimal balance = 0;

            foreach (var tx in Address.Transactions.OrderBy(p => p.Time))
            {
                var addressTx = new AddressTransaction()
                {
                    TotalIn = tx.TransactionsOut.Where(p => p.Address == Address.Id).Sum(x => x.Value),
                    TotalOut = tx.TransactionsIn.Where(p => p.PrevVOutFetchedAddress == Address.Id).Sum(x => x.PrevVOutFetchedValue),
                    Address = Address,
                    Transaction = tx
                };

                var amount = addressTx.TotalIn - addressTx.TotalOut;
                balance += amount;
                addressTx.Balance = balance;
                TotalReceived += addressTx.TotalIn;
                TotalSent += addressTx.TotalOut;
                AddressTransactions.Add(addressTx);
            }
        }
    }
}
