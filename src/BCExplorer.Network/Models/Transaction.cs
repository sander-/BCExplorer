using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCExplorer.Network.Models
{
    public class Transaction : BaseModel
    {
        public Block Block { get; set; }
        public TransactionType TransactionType { get;  set; }
        public string Blockhash { get;  set; }
        public string TransactionId { get;  set; }
        public uint Size { get;  set; }
        public List<TransactionIn> TransactionsIn { get;  set; }
        public List<TransactionOut> TransactionsOut { get;  set; }
        public object Time { get;  set; }

        public decimal TotalOut
        {
            get
            {
                if (TransactionsOut == null) return 0;

                return TransactionsOut.Sum(x => x.Value);
            }
        }

        public decimal Fees
        {
            get
            {
                if (TransactionType == TransactionType.PoS_Reward || TransactionType == TransactionType.PoW_Reward_Coinbase)
                    return 0;

                return TransactionsIn.Sum(x => x.PrevVOutFetchedValue) - TransactionsOut.Sum(x => x.Value);
            }
        }

        public string GetTransactionTypeText()
        {
            switch (TransactionType)
            {
                case TransactionType.PoW_Reward_Coinbase:
                    return "Proof-of-Work Miner Reward";
                case TransactionType.PoS_Reward:
                    return "Proof-of-Stake Block Reward";
                case TransactionType.Money:
                    return "Money Transfer";
                case TransactionType.Unknown:
                    return "Unknown";
                default:
                    return "Undefined";
            }
        }

        public class TransactionIn
        {
            /// <summary>
            /// Marks the first tx, The data in "coinbase" can be anything; it isn't used. 
            /// Bitcoin puts the current compact-format target and the arbitrary-precision "extraNonce" 
            /// number there, which increments every time the Nonce field in the block header overflows.
            /// https://en.bitcoin.it/wiki/Transaction#general_format_.28inside_a_block.29_of_each_input_of_a_transaction_-_Txin
            /// </summary>
            public string Coinbase { get; set; }
            public ulong Sequence { get; set; }
            public string PrevTxIdPointer { get; set; }
            public string PrevVOutFetchedAddress { get; set; }                        
            public int Index { get; set; }
            public decimal PrevVOutFetchedValue { get; set; }
            public string AssetId { get; set; }
            public int PrevVOutPointer { get; set; }
            public string ScriptSigHex { get; set; }
        }

        public class TransactionOut
        {
            public string TransactionId { get; set; }
            public decimal Value { get; set; }
            public string Address { get; set; }
            public int Index { get; set; }
            public string AssetId { get; set; }
            public long Quantity { get; set; }
        }
    }
}
