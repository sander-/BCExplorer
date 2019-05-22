using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Models
{
    public class AddressTransaction
    {
        public string TransactionId { get; set; }
        public string AddressId { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Time { get; set; }
    }
}
