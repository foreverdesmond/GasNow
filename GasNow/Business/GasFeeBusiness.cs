using System;
using GasNow.Module;
using GasNow.Dto;
using GasNow.Service;
using System.Text.Json;
using AutoMapper;
using NLog;
using StackExchange.Redis;

namespace GasNow.Business
{
    public class GasFeeBusiness
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private long _lastBlockNumber = 0;
        private GasNowDbContext _context;
        private IMapper _mapper;
        private IDatabase _redisDatabase;
        private static GasFeeDto _currentGasFeeDto;
        private static readonly object _lock = new object();

        public GasFeeBusiness()
        {
            CurrentGasFeeDto = new GasFeeDto();
        }

        public GasFeeBusiness(GasNowDbContext context)
        {
            CurrentGasFeeDto = new GasFeeDto();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GasFee, GasFeeDto>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _context = context;
        }

        public GasFeeBusiness(IConnectionMultiplexer redis)
        {
            CurrentGasFeeDto = new GasFeeDto();
            _redisDatabase = redis.GetDatabase();
        }

        public GasFeeBusiness(GasNowDbContext context, IConnectionMultiplexer redis)
        {
            CurrentGasFeeDto = new GasFeeDto();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GasFee, GasFeeDto>().ReverseMap();
            });

            _context = context;
            _mapper = config.CreateMapper();
            _redisDatabase = redis.GetDatabase();
        }

        public static GasFeeDto CurrentGasFeeDto
        {
            get
            {
                lock (_lock) 
                {
                    return _currentGasFeeDto;
                }
            }
            set
            {
                lock (_lock) 
                {
                    _currentGasFeeDto = value;
                }
            }
        }

        /// <summary>
        /// Asynchronously fetches gas fees from the specified API and saves them to the database and Redis.
        /// </summary>
        /// <param name="httpClient">The HttpClient used to make the API request.</param>
        /// <param name="apiUrl">The URL of the API to fetch gas fees from.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching or processing the gas fee data.</exception>
        public async Task GetAndSaveGasFeesAsync(HttpClient httpClient, string apiUrl)
        {
            try
            {
                var response = await httpClient.GetStringAsync(apiUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var gasData = JsonSerializer.Deserialize<GasApiResponse>(response, options);

                if (gasData != null && gasData.Result != null)
                {
                    var _currentBlockNumber = gasData.Result.LastBlock;
                    if (_currentBlockNumber > _lastBlockNumber)
                    {
                        _lastBlockNumber = _currentBlockNumber;

                        var congestionPercentage = GetCongestionPercentage(gasData.Result.GasUsedRatio);
                        var rapidGasPrice = CalculateRapidMaxFee(gasData.Result.FastGasPrice, congestionPercentage);

                        var gasFeeDto = new GasFeeDto
                        {
                            NetworkID = 1,
                            BlockNumber = _lastBlockNumber,
                            //BlockTimestamp = gasData.Result.BlockTimestamp,
                            SlowMaxFee = gasData.Result.SafeGasPrice,
                            SlowPriorityFee = gasData.Result.SafeGasPrice - gasData.Result.SuggestBaseFee,
                            NormalMaxFee = gasData.Result.ProposeGasPrice,
                            NormalPriorityFee = gasData.Result.ProposeGasPrice - gasData.Result.SuggestBaseFee,
                            FastMaxFee = gasData.Result.FastGasPrice,
                            FastPriorityFee = gasData.Result.FastGasPrice - gasData.Result.SuggestBaseFee,
                            RapidMaxFee = rapidGasPrice,
                            RapidPriorityFee = rapidGasPrice - gasData.Result.SuggestBaseFee,
                        };

                        CurrentGasFeeDto = gasFeeDto;

                        await SaveGasFeeToDatabase(gasFeeDto);

                        await _redisDatabase.StringSetAsync("CurrentGasFee", JsonSerializer.Serialize(gasFeeDto));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while fetching current price.");
                throw ex;
            }
        }

        /// <summary>
        /// Saves the provided gas fee data to the database.
        /// </summary>
        /// <param name="gasFeeDto">The gas fee data to save.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SaveGasFeeToDatabase(GasFeeDto gasFeeDto)
        {
            var gasFeeService = new GasFeeService(_context, _mapper);
            gasFeeService.AddGasFee(gasFeeDto);
        }

        /// <summary>
        /// Asynchronously retrieves the current gas fee from Redis.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation, containing the current gas fee data as a <see cref="GasFeeDto"/> object, or null if no data is found.
        /// </returns>
        public async Task<GasFeeDto> GetCurrentGasFee()
        {
            var json = await _redisDatabase.StringGetAsync("CurrentGasFee");

            if (json.IsNullOrEmpty)
            {
                Logger.Warn("No gas fee data found in Redis.");
                return null;
            }

            var gasFee = JsonSerializer.Deserialize<GasFeeDto>(json);

            return gasFee;
        }

        /// <summary>
        /// Converts a Unix timestamp to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="timestamp">The Unix timestamp to convert.</param>
        /// <returns>A <see cref="DateTimeOffset"/> representing the converted timestamp.</returns>
        public DateTimeOffset ConvertUnixTimeStamp(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp);
        }

        /// <summary>
        /// Calculates the congestion percentage based on the provided gas used ratio.
        /// </summary>
        /// <param name="gasUsedRatio">A comma-separated string representing the gas used ratio.</param>
        /// <returns>An integer representing the congestion percentage.</returns>
        /// <exception cref="ArgumentException">Thrown when the gas used ratio contains invalid numbers.</exception>
        public int GetCongestionPercentage(string gasUsedRatio)
        {
            var roundedRatios = gasUsedRatio
                .Split(',')
                .Select(s =>
                {
                    if (decimal.TryParse(s, out var number)) 
                    {
                        return Math.Round(number, 2);
                    }
                    throw new ArgumentException($"Invalid number in gasUsedRatio: {s}");
                })
                .ToArray();

            var average = roundedRatios.Average();

            return (int)(average * 100);
        }

        /// <summary>
        /// Calculates the rapid maximum fee based on the fast maximum fee and congestion level.
        /// </summary>
        /// <param name="fastMaxFee">The fast maximum fee.</param>
        /// <param name="congestion">The congestion level as a percentage.</param>
        /// <returns>The calculated rapid maximum fee.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the congestion level is not between 1 and 100.</exception>
        public static decimal CalculateRapidMaxFee(decimal fastMaxFee, int congestion)
        {
            if (congestion < 1 || congestion > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(congestion), "Congestion level must be between 1 and 100.");
            }

            decimal rapidMaxFee = fastMaxFee * (1.1M + (congestion - 1) * 0.009M);

            return rapidMaxFee;
        }
    }
}

