using System;
using System.Text.Json;
using AutoMapper;
using GasNow.Dto;
using GasNow.ExternalApis;
using GasNow.Helper;
using GasNow.Module;
using Microsoft.Extensions.Configuration;
using NLog;
using StackExchange.Redis;

namespace GasNow.Business
{
    public class GasFeeBlockNavieBusiness
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IDatabase _redisDatabase;
        private IConfiguration _configuration;
        private static GasFeeBlockNavieDto _currentGasFeeDto;
        private static readonly object _lock = new object();
        //private string _networkId;

        /// <summary>
        /// Gets or sets the current gas fee data.
        /// </summary>
        public static GasFeeBlockNavieDto CurrentGasFeeDto
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

        public GasFeeBlockNavieBusiness(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            CurrentGasFeeDto = new GasFeeBlockNavieDto();
            _redisDatabase = redis.GetDatabase();
            _configuration = configuration;
        }

        //public GasFeeBlockNavieBusiness(IConnectionMultiplexer redis, IConfiguration configuration, string networkId)
        //{
        //    CurrentGasFeeDto = new GasFeeBlockNavieDto();
        //    _redisDatabase = redis.GetDatabase();
        //    _configuration = configuration;
        //    _networkId = networkId;
        //}

        public async Task<GasFeeBlockNavieDto> GetCurrentGasFee(string networkId)
        {
            var json = await _redisDatabase.StringGetAsync("CurrentGasFee" + networkId);

            if (json.IsNullOrEmpty)
            {
                Logger.Warn("No gas fee data found in Redis.");
                return null;
            }

            var gasFee = JsonSerializer.Deserialize<GasFeeBlockNavieDto>(json);

            return gasFee;
        }

        public async Task GetAndSaveGasFeesAsync(int networkId)
        {
            if (networkId==0)
            {
                networkId = 1;
            }
            var blocknativeApiService = new BlocknativeApiService(_configuration, networkId);

            _currentGasFeeDto = await blocknativeApiService.GetCurrentGasFeeAsync();

            await _redisDatabase.StringSetAsync("CurrentGasFee" + networkId.ToString(), JsonSerializer.Serialize(_currentGasFeeDto)); ;
        }
    }
}

