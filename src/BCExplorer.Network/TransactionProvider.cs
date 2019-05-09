using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Network.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Network
{
    public class TransactionProvider : ITransactionProvider
    {
        readonly Rpc.IClient _client;
        const string NON_STANDARD = "nonstandard";

        public TransactionProvider(Rpc.IClient client)
        {
            _client = client;
        }

        public async Task<Transaction> GetTransaction(string id)
        {
            RawTransactionResult tx = await _client.GetRawTransactionAsync(id);

            if (tx == null)
                return null;

            TransactionType transactiontype = GetTransactionType(tx);

            var transaction = new Transaction
            {
                OriginalJson = tx.OriginalJson,
                //TransactionType = transactiontype,
                //Blockhash = tx.Blockhash,
                //TransactionId = tx.Txid,
                //Size = tx.Size,
                //TransactionIn = new List<VIn>(),
                //TransactionsOut = new List<Out>(),
                //Time = tx.GetTime()
            };

            return transaction;
        }


        static TransactionType GetTransactionType(RawTransactionResult tx)
        {
            //if (tx.Vin[0].Coinbase != null)
            //    return TransactionType.PoW_Reward_Coinbase;

            //var vout_0 = tx.Vout[0];
            //if (tx.Vout.Length == 3
            //    && vout_0.N == 0
            //    && vout_0.ScriptPubKey.Type == NON_STANDARD
            //    && string.IsNullOrEmpty(vout_0.ScriptPubKey.Hex))
            //    return TransactionType.PoS_Reward;

            return TransactionType.Money;
        }
    }
}
