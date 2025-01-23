using System;
using GasNow.Dto;
using GasNow.Module;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using NLog;
using Microsoft.Extensions.Configuration;
using GasNow.ExternalApis;

namespace GasNow.Business
{
	public class PriceBussiness
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private GasNowDbContext _context;
		private IMapper _mapper;
		private IDatabase _redisDatabase;
		private IConfiguration _configuration;
		private static PriceDto _currentPriceDto;
		private static readonly object _lock = new object();

		/// <summary>
		/// Gets or sets the current price data.
		/// </summary>
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
		/// Initializes a new instance of the <see cref="PriceBussiness"/> class.
		/// </summary>
		public PriceBussiness()
		{
			CurrentPriceDto = new PriceDto();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceBussiness"/> class with Redis connection.
		/// </summary>
		/// <param name="redis">The Redis connection multiplexer.</param>
		public PriceBussiness(IConnectionMultiplexer redis)
		{
			CurrentPriceDto = new PriceDto();
			_redisDatabase = redis.GetDatabase();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceBussiness"/> class with Redis connection and configuration.
		/// </summary>
		/// <param name="redis">The Redis connection multiplexer.</param>
		/// <param name="configuration">The application configuration.</param>
		public PriceBussiness(IConnectionMultiplexer redis, IConfiguration configuration)
		{
			CurrentPriceDto = new PriceDto();
			_redisDatabase = redis.GetDatabase();
			_configuration = configuration;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceBussiness"/> class with database context, Redis connection, and configuration.
		/// </summary>
		/// <param name="context">The database context.</param>
		/// <param name="redis">The Redis connection multiplexer.</param>
		/// <param name="configuration">The application configuration.</param>
		public PriceBussiness(GasNowDbContext context, IConnectionMultiplexer redis, IConfiguration configuration)
		{
			CurrentPriceDto = new PriceDto();
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Price, PriceDto>().ReverseMap();
			});

			_context = context;
			_mapper = config.CreateMapper();
			_redisDatabase = redis.GetDatabase();
			_configuration = configuration;
		}

		/// <summary>
		/// Asynchronously fetches the current price data from the specified API and saves it to Redis.
		/// </summary>
		/// <param name="configuration">The application configuration used to fetch price data.</param>
		/// <param name="saveToDatabase">Indicates whether to save the price data to the database.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		/// <exception cref="Exception">Thrown when an error occurs while fetching or processing the price data.</exception>
		public async Task GetAndSavePriceAsync(IConfiguration configuration, bool saveToDatabase)
		{
			var etherscanApiService = new EtherscanApiService(configuration);

			_currentPriceDto = await etherscanApiService.GetCurrentPriceAsync();

			await _redisDatabase.StringSetAsync("CurrentPrice", JsonSerializer.Serialize(_currentPriceDto));

			if (saveToDatabase)
			{
				// TODO: Implement saving to the database.
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

