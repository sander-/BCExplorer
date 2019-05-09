using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BCExplorer.Model
{
    public class Block
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int Height { get; set; }
        [MaxLength(64)]
        public string BlockHash { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string BlockData { get; set; }
    }
}
