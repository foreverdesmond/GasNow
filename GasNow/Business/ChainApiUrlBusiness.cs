using System;
using System.Collections.Generic;
using AutoMapper;
using GasNow.Dto;
using GasNow.Module;
using GasNow.Service;
using NLog;
using Microsoft.Extensions.Configuration;

namespace GasNow.Business
{
    public class ChainApiUrlBusiness
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ChainAPIUrlService _chainAPIUrlService;
        private readonly IMapper _mapper;
        private readonly GasNowDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainApiUrlBusiness"/> class with configuration.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public ChainApiUrlBusiness(IConfiguration configuration)
        {
            _configuration = configuration;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChainAPIUrl, ChainAPIUrlDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            _chainAPIUrlService = new ChainAPIUrlService(_context, _mapper);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainApiUrlBusiness"/> class with database context and configuration.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="configuration">The application configuration.</param>
        public ChainApiUrlBusiness(GasNowDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChainAPIUrl, ChainAPIUrlDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            _chainAPIUrlService = new ChainAPIUrlService(_context, _mapper);
        }

        /// <summary>
        /// Retrieves the <see cref="ChainAPIUrlDto"/> for a given API name.
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

        /// <summary>
        /// Retrieves the API URL as a string for a given API name from the configuration.
        /// </summary>
        /// <param name="apiName">The name of the API for which to retrieve the URL.</param>
        /// <param name="readConfig">A boolean indicating whether to read from the configuration.</param>
        /// <returns>
        /// A string representing the API URL from the configuration, or null if no URL is found.
        /// </returns>
        public string GetApiUrlByName(string apiName, bool readConfig)
        {
            if (readConfig)
            {
                return _configuration[$"ApiUrls:{apiName}"];
            }

            return GetApiUrlByName(apiName);
        }
    }
}
