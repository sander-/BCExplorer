﻿using System;
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
        public string[] TransactionIds { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}