using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCExplorer.Model
{
    public class Transaction
    {
        [MaxLength(64)]
        public string Id { get; set; }
        public Block Block { get; set; }
        public string TransactionData { get; set; }
    }
}
