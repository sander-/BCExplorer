using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BCExplorer.Network.Rpc
{
    public class ApplicationSettings<T> : IOptions<T> where T : class, new()
    {
        public ApplicationSettings() { }
        public T Value { get; set; }
    }

    public class RpcSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
    }
}
