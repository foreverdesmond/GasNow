using System;
using System.ComponentModel.DataAnnotations;

namespace GasNow.Module
{
	public class GasPriceAve
	{

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH-mm-ss zzz}")]
        public DateTimeOffset StartDateTimeOffset { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH-mm-ss zzz}")]
        public DateTimeOffset EndDateTimeOffset { get; set; }

	}
}

