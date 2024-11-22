using System;
using System.Collections.Generic;
using AutoMapper;
using GasNow.Dto;
using GasNow.Module;
using GasNow.Service;
using NLog;

namespace GasNow.Business
{
    public class ChainApiUrlBusiness
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ChainAPIUrlService _chainAPIUrlService;
        private readonly IMapper _mapper;
        private readonly GasNowDbContext _context;

        public ChainApiUrlBusiness(GasNowDbContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChainAPIUrl, ChainAPIUrlDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            _chainAPIUrlService = new ChainAPIUrlService(_context, _mapper);
        }

        /// <summary>
        /// Retrieves the ChainAPIUrlDto for a given API name.
        /// </summary>
        /// <param name="apiName">The name of the API for which to retrieve the URL.</param>
        /// <returns>
        /// A <see cref="ChainAPIUrlDto"/> object representing the API URL, or null if no URL is found.
        /// </returns>
        public ChainAPIUrlDto GetChainApiUrlByName(string apiName)
        {
            var apiUrls = _chainAPIUrlService.GetChainAPIUrlsByAPIName(apiName);
            if (apiUrls == null || apiUrls.Count == 0)
            {
                return null; 
            }

            return apiUrls[0]; 
        }

        /// <summary>
        /// Retrieves the API URL as a string for a given API name.
        /// </summary>
        /// <param name="apiName">The name of the API for which to retrieve the URL.</param>
        /// <returns>
        /// A string representing the API URL, or null if no URL is found.
        /// </returns>
        public string GetApiUrlByName(string apiName)
        {
            var apiUrls = _chainAPIUrlService.GetChainAPIUrlsByAPIName(apiName);
            if (apiUrls == null || apiUrls.Count == 0)
            {
                return null;
            }

            return apiUrls[0].APIUrl;
        }
    }
}
