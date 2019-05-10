using BCExplorer.Network.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Network.Rpc
{
    public class Client : IClient
    {
        readonly RpcSettings _settings;
        static ILogger _logger;

        public Client(RpcSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<RawTransactionResult> GetRawTransactionAsync(string txid, int jsonResult = 1)
        {
            try
            {
                var json = await InvokeMethod("getrawtransaction", txid, jsonResult);
                var response = JsonConvert.DeserializeObject<RpcResult<RawTransactionResult>>(json);
                if (response?.Result == null)
                    return null;
                response.Result.OriginalJson = json;
                return response.Result;
            }
            catch { }
            return null;
        }

        public async Task<string> GetBestBlockHashAsync()
        {
            var json = await InvokeMethod("getbestblockhash");
            var response = JsonConvert.DeserializeObject<RpcResult<string>>(json);
            return response.Result;
        }

        public async Task<BlockResult> GetBlockAsync(string hash)
        {
            var json = await InvokeMethod("getblock", hash);
            var response = JsonConvert.DeserializeObject<RpcResult<BlockResult>>(json);
            if (response?.Result == null)
                return null;
            response.Result.OriginalJson = json;
            return response.Result;
        }

        public async Task<string> GetBlockHashAsync(int blockNumber)
        {
            try
            {
                var json = await InvokeMethod("getblockhash", blockNumber);
                var result = JsonConvert.DeserializeObject<RpcResult<string>>(json);
                return result.Result;
            }
            catch (Exception e)
            {
                _logger.LogError($"getblockhash on {blockNumber} returned an error: {e.Message}.");
                return null;
            }
        }

        private async Task<string> InvokeMethod(string method, params object[] parameters)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_settings.Url);
            webRequest.Credentials = new NetworkCredential(_settings.User, _settings.Password);
            string username = _settings.User;
            string password = _settings.Password;
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

            webRequest.Headers["Authorization"] = "Basic " + credentials;
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            JObject json = new JObject();
            json["jsonrpc"] = "1.0";
            json["id"] = "1";
            json["method"] = method;

            if (parameters != null)
            {
                if (parameters.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (var p in parameters)
                    {
                        props.Add(p);
                    }
                    json.Add(new JProperty("params", props));
                }
            }

            string s = JsonConvert.SerializeObject(json);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);

            try
            {
                using (Stream dataStream = await webRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                using (WebResponse webResponse = await webRequest.GetResponseAsync())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return await sr.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting data: {e.Message}");
                throw;
            }
        }
    }
}
