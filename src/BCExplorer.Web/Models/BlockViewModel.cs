using BCExplorer.Network.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCExplorer.Web.Models
{
    public class BlockViewModel
    {
        public Block Block { get; set; }
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        public int OffSet { get; set; }
        public int Max { get; set; }
    }
}
