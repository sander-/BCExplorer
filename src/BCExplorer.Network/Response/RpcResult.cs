using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Response
{
    public class RpcResult<T>
    {
        [JsonProperty("result")]
        public T Result { get; set; }
        [JsonProperty("error")]
        public object Error { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
