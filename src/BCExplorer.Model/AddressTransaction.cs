using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BCExplorer.Model
{
    public class AddressTransaction
    {
        public int Id { get; set; }
        public Address Address { get; set; }
        [Column(TypeName = "decimal(18,8)")]
        public decimal Balance { get; set; }
        [Column(TypeName = "decimal(18,8)")]
        public decimal Amount { get; set; }
        [MaxLength(64)]
        public string TransactionId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
