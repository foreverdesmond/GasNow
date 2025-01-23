using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GasNow.Dto
{
	public class EstimatedPricesDto
	{
        [Range(0, 100, ErrorMessage = "Confidence must between 0 and 100.")]
        [JsonPropertyName("confidence")]
        public int confidence { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must larger than 0.")]
        [JsonPropertyName("price")]
        public decimal price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MaxPriorityFeePerGas must larger than 0.")]
        [JsonPropertyName("maxPriorityFeePerGas")]
        public decimal maxPriorityFeePerGas { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MaxFeePerGas must larger than 0.")]
        [JsonPropertyName("maxFeePerGas")]
        public decimal maxFeePerGas { get; set; }
    }
}

