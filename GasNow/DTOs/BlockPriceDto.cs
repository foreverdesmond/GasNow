using System;
using System.Text.Json.Serialization;
using System.Collections.Generic; 

namespace GasNow.Dto
{
	public class BlockPriceDto
    {
        [JsonPropertyName("blockNumber")]
        public long BlockNumber { get; set; }

        [JsonPropertyName("estimatedTransactionCount")]
        public int EstimatedTransactionCount { get; set; }

        //[JsonPropertyName("baseFeePerGas")]
        //public decimal BaseFeePerGas { get; set; }

        [JsonPropertyName("baseFeePerGas")]
        public string BaseFeePerGas { get; set; }

        [JsonPropertyName("estimatedPrices")]
        public List<EstimatedPricesDto> EstimatedPrices { get; set; }
	}
}

