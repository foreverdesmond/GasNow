using System.ComponentModel.DataAnnotations;

namespace GasNow.Dto
{
    public class GasFeeDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "BlockNumber must larger than 0.")]
        public long BlockNumber { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "NetworkID must larger than 0.")]
        public int NetworkID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "BlockTimestamp must larger than 0.")]
        public long BlockTimestamp { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SlowMaxFee must larger than 0.")]
        public decimal SlowMaxFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SlowPriorityFee must larger than 0.")]
        public decimal SlowPriorityFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "NormalMaxFee must larger than 0.")]
        public decimal NormalMaxFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "NormalPriorityFee must larger than 0.")]
        public decimal NormalPriorityFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "FastMaxFee must larger than 0.")]
        public decimal FastMaxFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "FastPriorityFee must larger than 0.")]
        public decimal FastPriorityFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "RapidMaxFee must larger than 0.")]
        public decimal RapidMaxFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "RapidPriorityFee must larger than 0.")]
        public decimal RapidPriorityFee { get; set; } 
    }
}
