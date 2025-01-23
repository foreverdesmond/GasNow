using System;
using System.ComponentModel.DataAnnotations;

namespace GasNow.Dto
{
    public class PriceDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "NetworkID must larger than 0.")]
        public int NetworkId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "EthBtcPrice must larger than 0.")]
        public decimal EthBtcPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "EthBtcTimestamp must larger than 0.")]
        public long EthBtcTimestamp { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "EthUsdPrice must larger than 0.")]
        public decimal EthUsdPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "EthUsdTimestamp must larger than 0.")]
        public long EthUsdTimestamp { get; set; }
    }
}

