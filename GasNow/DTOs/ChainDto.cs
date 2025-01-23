using System.ComponentModel.DataAnnotations;

namespace GasNow.Dto
{
    public class ChainDto
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "NetworkID must larger than 0.")]
        public int NetworkID { get; set; }

        [StringLength(20)]
        public string? ChainName { get; set; }

        [StringLength(10)]
        public string? Currency { get; set; }

        [StringLength(100)]
        public string? Explorer { get; set; }
    }
}
