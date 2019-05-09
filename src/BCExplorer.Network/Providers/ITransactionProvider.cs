using BCExplorer.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Network.Providers
{
    public interface ITransactionProvider
    {
        Task<Transaction> GetTransaction(string id);
    }
}
