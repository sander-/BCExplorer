using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using BCExplorer.Network.Extensions;

namespace BCExplorer.Network.Response
{
    public class RawTransactionResult : BaseResult
    {
        [JsonProperty("hex")]
        public string Hex { get; set; }
        [JsonProperty("txid")]
        public string Txid { get; set; }
        [JsonProperty("size")]
        public uint Size { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("locktime")]
        public int Locktime { get; set; }
        [JsonProperty("vin")]
        public IList<Vin> Input { get; set; }
        [JsonProperty("vout")]
        public IList<Vout> Output { get; set; }
        [JsonProperty("blockhash")]
        public string Blockhash { get; set; }
        [JsonProperty("height")]
        public uint Height { get; set; }
        [JsonProperty("confirmations")]
        public uint Confirmations { get; set; }
        [JsonProperty("time")]
        public uint Time { get; set; }
        [JsonProperty("blocktime")]
        public uint Blocktime { get; set; }

        public DateTime GetTime()
        {
            return Time.FromUnixDateTime();
        }

        public class ScriptPubKey
        {
            [JsonProperty("asm")]
            public string Asm { get; set; }
            [JsonProperty("hex")]
            public string Hex { get; set; }
            [JsonProperty("reqSigs")]
            public int ReqSigs { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("addresses")]
            public IList<string> Addresses { get; set; }
        }

        public class ScriptSig
        {
            [JsonProperty("asm")]
            public string Asm { get; set; }
            [JsonProperty("hex")]
            public string Hex { get; set; }
        }

        public class Vout
        {
            [JsonProperty("value")]
            public decimal Value { get; set; }
            [JsonProperty("valueSat")]
            public long ValueSat { get; set; }
            [JsonProperty("n")]
            public int Index { get; set; }
            [JsonProperty("scriptPubKey")]
            public ScriptPubKey ScriptPubKey { get; set; }
        }

        public class Vin
        {
            [JsonProperty("txid")]
            public string Txid { get; set; }
            [JsonProperty("coinbase")]
            public string Coinbase { get; set; }
            [JsonProperty("sequence")]
            public ulong Sequence { get; set; }
            [JsonProperty("scriptSig")]
            public ScriptSig ScriptSig { get; set; }
            [JsonProperty("vout")]
            public ulong Vout { get; set; }
        }
    }
}
