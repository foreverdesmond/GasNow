using System;
using System.Text.Json.Serialization;

namespace GasNow.Module
{
    public class JsonRpcResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("result")]
        public object Result { get; set; }
    }
}