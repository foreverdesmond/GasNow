using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasNow.Module
{
    [Table("chain")]
    public class Chain
	{
		[Key]
        [Column("network_id")]
        public required int NetworkID { get; set; }

        [Column("chain_name")]
        public required string ChainName { get; set; }
		
        [Column("currency")]
        public required string Currency { get; set; }

        [Column("explorer")]
        public required string Explorer { get; set; }

    }
}

