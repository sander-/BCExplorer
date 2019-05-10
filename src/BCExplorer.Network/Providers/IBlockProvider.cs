using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Network.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Network.Providers
{
    public interface IBlockProvider
    {
        Task<Block> GetBlock(string id);
        Task<string> GetBestBlockHash();
    }
}
