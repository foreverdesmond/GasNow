using System;
using GasNow.Dto;
using GasNow.Module;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using NLog;

namespace GasNow.Business
{
	public class PriceBussiness
	{

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private GasNowDbContext _context;
        private IMapper _mapper;
        private IDatabase _redisDatabase;
        private static PriceDto _currentPriceDto;
        private static readonly object _lock = new object();

        public PriceBussiness()
        {
            CurrentPriceDto = new PriceDto();
        }

        public PriceBussiness(GasNowDbContext context)
        {
            CurrentPriceDto = new PriceDto();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Price, PriceDto>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _context = context;
        }

        public PriceBussiness(IConnectionMultiplexer redis)
        {
            CurrentPriceDto = new PriceDto();
            _redisDatabase = redis.GetDatabase();
        }

        public PriceBussiness(GasNowDbContext context, IConnectionMultiplexer redis)
        {
            CurrentPriceDto = new PriceDto();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Price, PriceDto>().ReverseMap();
            });

            _context = context;
            _mapper = config.CreateMapper();
            _redisDatabase = redis.GetDatabase();
        }

        public static PriceDto CurrentPriceDto
        {
            get
            {
                lock (_lock)
                {
                    return _currentPriceDto;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentPriceDto = value;
                }
            }
        }

        /// <summary>
        /// Asynchronously fetches the current price data from the specified API and saves it to Redis.
        /// </summary>
        /// <param name="httpClient">The HttpClient used to make the API request.</param>
        /// <param name="apiUrl">The URL of the API to fetch price data from.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching or processing the price data.</exception>
        public async Task GetPrice(HttpClient httpClient, string apiUrl)
        {
            try
            {
                var response = await httpClient.GetStringAsync(apiUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var priceData = JsonSerializer.Deserialize<PriceApiResponse>(response, options);

                if (priceData != null && priceData.Result != null)
                {

                    var priceDto = new PriceDto
                    {
                        NetworkId = 1,
                        EthBtcPrice = priceData.Result.EthBtc,
                        EthBtcTimestamp = priceData.Result.EthBtcTimestamp,
                        EthUsdPrice = priceData.Result.EthUsd,
                        EthUsdTimestamp = priceData.Result.EthUsdTimestamp
                    };

                    await _redisDatabase.StringSetAsync("CurrentPrice", JsonSerializer.Serialize(priceDto));
                }
                else
                {
                    Logger.Warn("Invalid response or status not OK.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while fetching current price.");
                throw ex;
            }
        }

        /// <summary>
        /// Asynchronously retrieves the current price data from Redis.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation, containing the current price data as a <see cref="PriceDto"/> object, or null if no data is found.
        /// </returns>
        public async Task<PriceDto> GetCurrentPrice()
        {
            var json = await _redisDatabase.StringGetAsync("CurrentPrice");


            if (json.IsNullOrEmpty)
            {
                Logger.Warn("No price data found in Redis.");
                return null;
            }

            var price = JsonSerializer.Deserialize<PriceDto>(json);

            return price;
        }
    }
}

