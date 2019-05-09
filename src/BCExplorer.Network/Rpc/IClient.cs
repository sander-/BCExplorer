using BCExplorer.Network.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Network.Rpc
{
    public interface IClient
    {
        Task<RawTransactionResult> GetRawTransactionAsync(string txid, int jsonResult = 1);
    }
}
