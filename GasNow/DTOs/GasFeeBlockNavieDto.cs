using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic; 

namespace GasNow.Dto
{
    public class GasFeeBlockNavieDto
    {
        [JsonPropertyName("system")]
        public string System { get; set; }

        [JsonPropertyName("network")]
        public string Network { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("maxPrice")]
        public decimal MaxPrice { get; set; }

        [JsonPropertyName("currentBlockNumber")]
        public long CurrentBlockNumber { get; set; }

        [JsonPropertyName("msSinceLastBlock")]
        public int MsSinceLastBlock { get; set; }

        [JsonPropertyName("blockPrices")]
        public List<BlockPriceDto> BlockPrices { get; set; }
    }
}

