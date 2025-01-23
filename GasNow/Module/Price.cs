using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasNow.Module
{
    [Table("price")]
    public class Price
    {
        [Column("networkid")]
        public int NetworkId { get; set; }

        [Column("eth_btc_price")]
        public decimal EthBtcPrice { get; set; }

        [Column("eth_btc_timestamp")]
        public long EthBtcTimestamp { get; set; }

        [Column("eth_usd_price")]
        public decimal EthUsdPrice { get; set; }

        [Column("eth_usd_timestamp")]
        public long EthUsdTimestamp { get; set; }
    }
}

