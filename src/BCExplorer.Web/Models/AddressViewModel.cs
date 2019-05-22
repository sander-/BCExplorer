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

        public void CalculateTotals()
        {
            foreach (var tx in Address.Transactions)
            {
                if (tx.Amount > 0) TotalReceived += tx.Amount;
                if (tx.Amount < 0) TotalSent += tx.Amount;
            }
        }
    }
}
