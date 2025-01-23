using System;
using System.Text.Json.Serialization;

namespace GasNow.Module
{
    public class PriceApiResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("result")]
        public PriceResult? Result { get; set; }
    }
}

