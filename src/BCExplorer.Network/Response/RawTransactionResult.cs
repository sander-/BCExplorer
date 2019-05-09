using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Response
{
    public class RawTransactionResult : BaseResult
    {
        public string hex { get; set; }
        public string txid { get; set; }
        public int size { get; set; }
        public int version { get; set; }
        public int locktime { get; set; }
        public IList<Vin> vin { get; set; }
        public IList<Vout> vout { get; set; }
        public string blockhash { get; set; }
        public int height { get; set; }
        public int confirmations { get; set; }
        public int time { get; set; }
        public int blocktime { get; set; }

        public class Vin
        {
            public string coinbase { get; set; }
            public long sequence { get; set; }
        }

        public class ScriptPubKey
        {
            public string asm { get; set; }
            public string hex { get; set; }
            public int reqSigs { get; set; }
            public string type { get; set; }
            public IList<string> addresses { get; set; }
        }

        public class Vout
        {
            public double value { get; set; }
            public long valueSat { get; set; }
            public int n { get; set; }
            public ScriptPubKey scriptPubKey { get; set; }
        }
    }
}
