using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Models
{
    public class Transaction : BaseModel
    {
        public Block Block { get; set; }
    }
}
