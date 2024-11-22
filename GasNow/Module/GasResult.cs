using System;
using GasNow.Helper;
using System.Text.Json.Serialization;

namespace GasNow.Module
{
    public class GasResult
    {
        [JsonPropertyName("LastBlock")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int LastBlock { get; set; }

        [JsonPropertyName("SafeGasPrice")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal SafeGasPrice { get; set; }

        [JsonPropertyName("ProposeGasPrice")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal ProposeGasPrice { get; set; }

        [JsonPropertyName("FastGasPrice")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal FastGasPrice { get; set; }

        [JsonPropertyName("suggestBaseFee")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal SuggestBaseFee { get; set; }

        [JsonPropertyName("gasUsedRatio")]
        public string GasUsedRatio { get; set; }
    }
}

