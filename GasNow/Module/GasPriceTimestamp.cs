using System;
using System.ComponentModel.DataAnnotations;

namespace GasNow.Module
{
	public class GasPriceTimestamp :GasPrice
	{
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH-mm-ss zzz}")]
		public DateTimeOffset Timestamp { get; set; }

		[Required]
		[Range(0,int.MaxValue,ErrorMessage ="Height must larger than 0.")]
		public int Height { get; set; }

    }
}

