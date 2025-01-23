using AutoMapper;
using GasNow.Dto;

namespace GasNow.Module
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Chain, ChainDto>().ReverseMap();
            CreateMap<ChainAPIUrl, ChainAPIUrlDto>().ReverseMap();
            CreateMap<GasFee,GasFeeDto>().ReverseMap();
            CreateMap<Price,PriceDto>().ReverseMap();
        }
    }
}
