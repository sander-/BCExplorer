using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BCExplorer.Model
{
    public class Address
    {
        [MaxLength(34)]
        public string Id { get; set; }
        [Column(TypeName = "decimal(18,8)")]
        public decimal Balance { get; set; }
        public string TxIdBlob { get; set; }
        public int LastModifiedBlockHeight { get; set; }
    }
}
