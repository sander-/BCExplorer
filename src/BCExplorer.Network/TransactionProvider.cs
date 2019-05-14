using BCExplorer.Network.Models;
using BCExplorer.Network.Providers;
using BCExplorer.Network.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin.DataEncoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Network
{
    public class TransactionProvider : ITransactionProvider
    {
        readonly Rpc.IClient _client;
        public const string NON_STANDARD = "nonstandard";

        public TransactionProvider(IOptions<Rpc.RpcSettings> options, ILogger logger)
        {
            _client = new Rpc.Client(options.Value, logger);
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
                TransactionType = transactiontype,
                Blockhash = tx.Blockhash,
                TransactionId = tx.Txid,
                Size = tx.Size,
                TransactionsIn = new List<Transaction.TransactionIn>(),
                TransactionsOut = new List<Transaction.TransactionOut>(),
                Time = tx.GetTime()
            };

            int index = 0;
            foreach (var input in tx.Input)
            {
                var vIn = new Transaction.TransactionIn
                {
                    Index = index,
                    Coinbase = input.Coinbase,
                    Sequence = input.Sequence,
                    ScriptSigHex = input.ScriptSig?.Hex,
                    AssetId = null,
                    // pointer to previous tx/vout:
                    PrevTxIdPointer = input.Txid,
                    PrevVOutPointer = (int)input.Vout,
                    // we'll try to fetch this id possible
                    PrevVOutFetchedAddress = null,
                    PrevVOutFetchedValue = 0
                };

                if (input.Txid != null)
                {
                    // Retrieve the origin address by retrieving the previous transaction and extracting the receive address and value
                    var previousTx = await _client.GetRawTransactionAsync(input.Txid);
                    if (previousTx != null)
                    {
                        var n = input.Vout;
                        vIn.PrevVOutFetchedAddress = previousTx.Output[(int)n].ScriptPubKey.Addresses.First();
                        vIn.PrevVOutFetchedValue = previousTx.Output[(int)n].Value;
                    }
                }
                transaction.TransactionsIn.Add(vIn);
            }

            index = 0;
            foreach (var output in tx.Output)
            {
                var vOut = new Transaction.TransactionOut
                {
                    TransactionId = transaction.TransactionId,
                    Value = output.Value,
                    Quantity = output.Index,
                    AssetId = null,
                    Index = index++,
                };

                if (output.ScriptPubKey.Addresses != null) // Satoshi 14.2
                    vOut.Address = output.ScriptPubKey.Addresses.FirstOrDefault();
                else
                {
                    string hexScript = output.ScriptPubKey.Hex;

                    if (!string.IsNullOrEmpty(hexScript))
                    {
                        byte[] decodedScript = Encoders.Hex.DecodeData(hexScript);
                        NBitcoin.Script script = new NBitcoin.Script(decodedScript);
                        var pubKey = NBitcoin.PayToPubkeyTemplate.Instance.ExtractScriptPubKeyParameters(script);
                        if (pubKey != null)
                        {
                            NBitcoin.BitcoinPubKeyAddress address = pubKey.GetAddress(NetworkSpecs.Gravium.Main());
                            vOut.Address = address.ToString();
                        }
                        else
                        {
                            vOut.Address = script.ToString();
                        }

                    }
                    else
                    {
                        vOut.Address = output.ScriptPubKey.Type;
                    }
                }
                transaction.TransactionsOut.Add(vOut);
            }

            return transaction;
        }


        static TransactionType GetTransactionType(RawTransactionResult tx)
        {
            if (tx.Input[0].Coinbase != null)
                return TransactionType.PoW_Reward_Coinbase;

            var vout_0 = tx.Output[0];
            if (tx.Output.Count == 3
                && vout_0.Index == 0
                && vout_0.ScriptPubKey.Type == NON_STANDARD
                && string.IsNullOrEmpty(vout_0.ScriptPubKey.Hex))
                return TransactionType.PoS_Reward;

            return TransactionType.Money;
        }
    }
}
