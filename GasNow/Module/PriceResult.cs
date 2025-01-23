using System;
using GasNow.Helper;
using System.Text.Json.Serialization;

namespace GasNow.Module
{
    public class PriceResult
    {
        [JsonPropertyName("ethbtc")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal EthBtc { get; set; }

        [JsonPropertyName("ethbtc_timestamp")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long EthBtcTimestamp { get; set; }

        [JsonPropertyName("ethusd")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal EthUsd { get; set; }

        [JsonPropertyName("ethusd_timestamp")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long EthUsdTimestamp { get; set; }
    }
}

