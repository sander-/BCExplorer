using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using BCExplorer.Network.Extensions;

namespace BCExplorer.Network.Response
{
    public class BlockResult : BaseResult
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("Confirmations")]
        public int Confirmations { get; set; }
        [JsonProperty("Size")]
        public int Size { get; set; }
        [JsonProperty("Height")]
        public int Height { get; set; }
        [JsonProperty("Version")]
        public int Version { get; set; }
        [JsonProperty("Merkleroot")]
        public string Merkleroot { get; set; }
        [JsonProperty("tx")]
        public IList<string> Transactions { get; set; }
        [JsonProperty("Time")]
        public uint Time { get; set; }
        [JsonProperty("Mediantime")]
        public int Mediantime { get; set; }
        [JsonProperty("Nonce")]
        public long Nonce { get; set; }
        [JsonProperty("Bits")]
        public string Bits { get; set; }
        [JsonProperty("Difficulty")]
        public double Difficulty { get; set; }
        [JsonProperty("Chainwork")]
        public string Chainwork { get; set; }
        [JsonProperty("Previousblockhash")]
        public string Previousblockhash { get; set; }
        [JsonProperty("Nextblockhash")]
        public string Nextblockhash { get; set; }
        public DateTime GetTime()
        {
            return Time.FromUnixDateTime();
        }
        public TimeSpan GetAge()
        {
            return DateTime.Now.Subtract(GetTime());
        }
    }
}
