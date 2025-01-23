using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasNow.Dto
{
    
    public class ChainAPIUrlDto
    {
        [Key]
        public Guid GUID { get; set; }
        
        public string? APIName { get; set; }
        
        public string? APIUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "NetworkID must larger than 0.")]
        public int NetworkID { get; set; }
    }
}
