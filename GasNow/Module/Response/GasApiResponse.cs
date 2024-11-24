using System;
using System.Text.Json.Serialization;

namespace GasNow.Module
{
    public class GasApiResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("result")]
        public GasResult? Result { get; set; }
    }
   
}

