using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasNow.Module
{
    [Table("gas_fee")]
    public class GasFee
    {
        [Column("block_number")]
        public long BlockNumber { get; set; }

        [Column("network_id")]
        public int NetworkID { get; set; }

        [Column("block_timestamp")]
        public long BlockTimestamp { get; set; }

        [Column("slow_max_fee")]
        public decimal SlowMaxFee { get; set; }

        [Column("slow_priority_fee")]
        public decimal SlowPriorityFee { get; set; }

        [Column("normal_max_fee")]
        public decimal NormalMaxFee { get; set; }

        [Column("normal_priority_fee")]
        public decimal NormalPriorityFee { get; set; }

        [Column("fast_max_fee")]
        public decimal FastMaxFee { get; set; }

        [Column("fast_priority_fee")]
        public decimal FastPriorityFee { get; set; }

        [Column("rapid_max_fee")]
        public decimal RapidMaxFee { get; set; }

        [Column("rapid_priority_fee")]
        public decimal RapidPriorityFee { get; set; }
    }
}

