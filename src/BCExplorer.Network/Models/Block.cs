using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Models
{
    public class Block
    {
        public string Hash { get; set; }
        public long Height { get; set; }
        public DateTime Time { get; set; }
        public long Confirmations { get; set; }
        public double Difficulty { get; set; }
        public string MerkleRoot { get; set; }
        public long Nonce { get; set; }
        public int TotalTransactions { get; set; }
        public string PreviousBlock { get; set; }
        public string NextBlock { get; set; }
        public IList<Transaction> Transactions { get; set; }
        public bool IsProofOfStake { get; set; }
        public string Chainwork { get; set; }
        public string Bits { get; set; }
        public string VersionHex { get; set; }
        public uint Weight { get; set; }
        public int Size { get; set; }
        public int StrippedSize { get; set; }
        public uint Version { get; set; }
        public TimeSpan Age { get; set; }
        public IList<string> TransactionIds { get; internal set; }
    }
}
