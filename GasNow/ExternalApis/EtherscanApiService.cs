using System.Text.Json;
using GasNow.Dto;
using GasNow.Module;
using GasNow.Helper;
using Microsoft.Extensions.Configuration;
using NLog;
using DotNetEnv;

namespace GasNow.ExternalApis
{


    public class EtherscanApiService
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _apiUrl; // The base URL for the Etherscan API.
        private readonly string _apiKey; // The API key for authenticating requests.
        private readonly HttpClient _httpClient; // The HTTP client used to make requests.
        private readonly GasCalculate _gasCalculate; // Utility for gas calculations.

        /// <summary>
        /// Initializes a new instance of the <see cref="EtherscanApiService"/> class with the specified configuration.
        /// </summary>
        /// <param name="configuration">The application configuration containing API URL and key.</param>
        public EtherscanApiService(IConfiguration configuration)
		{
			Env.Load();
			_apiUrl = configuration["ApiUrls:EtherscanApi"];
			_apiKey = Environment.GetEnvironmentVariable("ETHERSCAN_API_KEY");
			_httpClient = new HttpClient();
			_gasCalculate = new GasCalculate();
		}

		/// <summary>
		/// Asynchronously retrieves the current gas fee from the Etherscan API.
		/// </summary>
		/// <returns>
		/// A task representing the asynchronous operation, containing the current gas fee as a <see cref="GasFeeDto"/> object.
		/// </returns>
		/// <exception cref="Exception">Thrown when an error occurs while fetching or processing the gas fee data.</exception>
		public async Task<GasFeeDto> GetCurrentGasFeeAsync()
		{
			var apiUrl = _apiUrl + "?module=gastracker&action=gasoracle&apikey=" + _apiKey;
			string response = string.Empty;

			try
			{
				response = await _httpClient.GetStringAsync(apiUrl);

				var options = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};

				var gasData = JsonSerializer.Deserialize<GasApiResponse>(response, options);

				if (gasData == null)
				{
					Logger.Warn("Gas data is null. Response: {Response}", response);
					return null;
				}

				if (gasData.Result == null)
				{
					Logger.Warn("Gas data result is null. Response: {Response}", response);
					return null;
				}

				if (gasData.Status != "1")
				{
					Logger.Warn("Received gas data with status: {Status}. Response: {Response}", gasData.Status, response);
					return null;
				}

				var congestionPercentage = _gasCalculate.GetCongestionPercentage(gasData.Result.GasUsedRatio);
				var rapidGasPrice = _gasCalculate.CalculateRapidMaxFee(gasData.Result.FastGasPrice, congestionPercentage);
				var gasFeeDto = new GasFeeDto
				{
					NetworkID = 1,
					BlockNumber = gasData.Result.LastBlock,
					SlowMaxFee = gasData.Result.SafeGasPrice,
					SlowPriorityFee = gasData.Result.SafeGasPrice - gasData.Result.SuggestBaseFee,
					NormalMaxFee = gasData.Result.ProposeGasPrice,
					NormalPriorityFee = gasData.Result.ProposeGasPrice - gasData.Result.SuggestBaseFee,
					FastMaxFee = gasData.Result.FastGasPrice,
					FastPriorityFee = gasData.Result.FastGasPrice - gasData.Result.SuggestBaseFee,
					RapidMaxFee = rapidGasPrice,
					RapidPriorityFee = rapidGasPrice - gasData.Result.SuggestBaseFee,
				};
				return gasFeeDto;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "An error occurred while fetching current gas fee.");
				return new GasFeeDto();
			}
		}

		/// <summary>
		/// Asynchronously retrieves the current Ethereum price from the Etherscan API.
		/// </summary>
		/// <returns>
		/// A task representing the asynchronous operation, containing the current price as a <see cref="PriceDto"/> object.
		/// </returns>
		/// <exception cref="Exception">Thrown when an error occurs while fetching or processing the price data.</exception>
		public async Task<PriceDto> GetCurrentPriceAsync()
		{
			var apiUrl = _apiUrl + "&module=stats&action=ethprice&apikey=" + _apiKey;
			string response = string.Empty;

			try
			{
				response = await _httpClient.GetStringAsync(apiUrl);

				var options = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};

				var priceData = JsonSerializer.Deserialize<PriceApiResponse>(response, options);

				if (priceData == null)
				{
					Logger.Warn("Price data is null. Response: {Response}", response);
					return new PriceDto();
				}

				if (priceData.Result == null)
				{
					Logger.Warn("Price data result is null. Response: {Response}", response);
					return new PriceDto();
				}

				if (priceData.Status != "1")
				{
					Logger.Warn("Received price data with status: {Status}. Response: {Response}", priceData.Status, response);
					return new PriceDto();
				}

				var priceDto = new PriceDto
				{
					NetworkId = 1,
					EthBtcPrice = priceData.Result.EthBtc,
					EthBtcTimestamp = priceData.Result.EthBtcTimestamp,
					EthUsdPrice = priceData.Result.EthUsd,
					EthUsdTimestamp = priceData.Result.EthUsdTimestamp
				};

				return priceDto;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "An error occurred while fetching current price.");
				return new PriceDto();
			}
		}
	}
}

