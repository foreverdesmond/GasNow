using GasNow.Module;
using GasNow.Dto;
using GasNow.Service;
using GasNow.Helper;
using GasNow.ExternalApis;
using System.Text.Json;
using AutoMapper;
using NLog;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace GasNow.Business
{
    public class GasFeeBusiness
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private long _lastBlockNumber = 0;
        private GasNowDbContext _context;
        private IMapper _mapper;
        private IDatabase _redisDatabase;
        private IConfiguration _configuration;
        private static GasFeeDto _currentGasFeeDto;
        private static readonly object _lock = new object();
        private GasCalculate _gasCalculate;

        /// <summary>
        /// Gets or sets the current gas fee data.
        /// </summary>
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
        /// Initializes a new instance of the <see cref="GasFeeBusiness"/> class.
        /// </summary>
        public GasFeeBusiness()
        {
            CurrentGasFeeDto = new GasFeeDto();
            _gasCalculate = new GasCalculate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GasFeeBusiness"/> class with Redis connection.
        /// </summary>
        /// <param name="redis">The Redis connection multiplexer.</param>
        public GasFeeBusiness(IConnectionMultiplexer redis)
        {
            CurrentGasFeeDto = new GasFeeDto();
            _redisDatabase = redis.GetDatabase();
            _gasCalculate = new GasCalculate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GasFeeBusiness"/> class with Redis connection and configuration.
        /// </summary>
        /// <param name="redis">The Redis connection multiplexer.</param>
        /// <param name="configuration">The application configuration.</param>
        public GasFeeBusiness(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            CurrentGasFeeDto = new GasFeeDto();
            _redisDatabase = redis.GetDatabase();
            _configuration = configuration;
            _gasCalculate = new GasCalculate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GasFeeBusiness"/> class with database context, Redis connection, and configuration.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="redis">The Redis connection multiplexer.</param>
        /// <param name="configuration">The application configuration.</param>
        public GasFeeBusiness(GasNowDbContext context, IConnectionMultiplexer redis, IConfiguration configuration)
        {
            CurrentGasFeeDto = new GasFeeDto();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GasFee, GasFeeDto>().ReverseMap();
            });

            _context = context;
            _mapper = config.CreateMapper();
            _redisDatabase = redis.GetDatabase();
            _configuration = configuration;
            _gasCalculate = new GasCalculate();
        }

        /// <summary>
        /// Asynchronously retrieves the current gas fee and saves it to Redis and optionally to the database.
        /// </summary>
        /// <param name="saveToDatabase">Indicates whether to save the gas fee data to the database.</param>
        public async Task GetAndSaveGasFeesAsync(bool saveToDatabase)
        {
            var etherscanApiService = new EtherscanApiService(_configuration);

            _currentGasFeeDto = await etherscanApiService.GetCurrentGasFeeAsync();

            var _currentBlockNumber = _currentGasFeeDto.BlockNumber;
            if (_currentBlockNumber > _lastBlockNumber)
            {
                _lastBlockNumber = _currentBlockNumber;
                CurrentGasFeeDto = _currentGasFeeDto;

                await _redisDatabase.StringSetAsync("CurrentGasFee", JsonSerializer.Serialize(CurrentGasFeeDto));

                if (saveToDatabase)
                {
                    SaveGasFeeToDatabase(CurrentGasFeeDto);
                }
            }
        }

        /// <summary>
        /// Saves the provided gas fee data to the database.
        /// </summary>
        /// <param name="gasFeeDto">The gas fee data to save.</param>
        private void SaveGasFeeToDatabase(GasFeeDto gasFeeDto)
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
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }

        /// <summary>
        /// Asynchronously retrieves the average gas fee for the last week.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a list of average gas fees for each day of the last week.</returns>
        public async Task<List<decimal>> GetGasFeeLastWeek()
        {
            var blockNumbersLastWeek = await GetBlockNumbersLastWeek();

            var baseFeeInAWeek = new List<decimal>();

            var infuraApiService = new InfuraApiService(_configuration);

            for (int day = 0; day < 7; day++)
            {
                List<decimal> baseFeeInADay = new List<decimal>();

                for (int hour = 0; hour < 24; hour++)
                {
                    try
                    {
                        long blockNumber = blockNumbersLastWeek[day, hour];

                        var baseFeeList = await infuraApiService.GetBaseFeeHistory(blockNumber);

                        baseFeeInADay.AddRange(baseFeeList);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to get baseFeeHistory for day {day} hour {hour}: {ex}");
                    }
                }

                decimal baseFeeDayAll = 0;

                foreach (decimal baseFee in baseFeeInADay)
                {
                    baseFeeDayAll += baseFee;
                }

                var baseFeeDayAve = baseFeeDayAll / (baseFeeInADay.Count > 0 ? baseFeeInADay.Count : 1);

                baseFeeInAWeek.Add(baseFeeDayAve);
            }

            return baseFeeInAWeek;
        }

        /// <summary>
        /// Asynchronously retrieves the block numbers for the last week.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a 2D array of block numbers for the last week.</returns>
        public async Task<long[,]> GetBlockNumbersLastWeek()
        {
            var etherscanApiService = new EtherscanApiService(_configuration);
            var currentGasFeeDto = await etherscanApiService.GetCurrentGasFeeAsync();
            var currentBlockNumber = currentGasFeeDto.BlockNumber;

            const int blocksPerHour = 300;
            var blockNumbersLastWeek = new long[7, 24];

            var now = DateTime.UtcNow;

            var startOfToday = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            var totalSeconds = (now - startOfToday).TotalSeconds;

            long blockNumberTodayBegin = currentBlockNumber - (long)(totalSeconds / 12);

            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                var targetDate = startOfToday.AddDays(-dayOffset);

                for (int hour = 0; hour < 24; hour++)
                {
                    blockNumbersLastWeek[dayOffset, hour] = blockNumberTodayBegin - (dayOffset * 24 * blocksPerHour) - (hour * blocksPerHour);
                }
            }

            return blockNumbersLastWeek;
        }
    }
}

