using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Models
{
    public class Address
    {
        public string Id { get; set; }
        public decimal Balance { get; set; }
        public int LastModifiedBlockHeight { get; set; }
        public int TotalTransactions { get; set; }        
        public IList<AddressTransaction> Transactions { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal TotalSent { get; set; }
    }
}
