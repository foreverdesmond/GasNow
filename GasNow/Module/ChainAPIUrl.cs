using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasNow.Module
{
    [Table("api_url")]
    public class ChainAPIUrl
	{
		[Key]
		[Required]
        [Column("guid")]
        public required Guid Guid { get; set; }

		[Required]
        [Column("api_name")]
        public required string APIName { get; set; }

		[Required]
        [Column("api_url")]
        public required string APIUrl { get; set; }

		[Required]
        [Column("network_id")]
        public required int NetworkID { get; set; }

	}
}

