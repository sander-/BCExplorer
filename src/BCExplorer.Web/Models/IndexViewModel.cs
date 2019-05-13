using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCExplorer.Network.Models;

namespace BCExplorer.Web.Models
{
    public class IndexViewModel
    {
        public Block LastBlock { get; set; }

        public List<Block> LatestBlocks { get; set; }
    }
}
