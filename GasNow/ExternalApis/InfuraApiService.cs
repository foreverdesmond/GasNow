using GasNow.Module;
using System.Text.Json;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.Numerics;
using NLog;

namespace GasNow.ExternalApis
{
	public class InfuraApiService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly string _apiUrl; // The base URL for the Infura API.
		private readonly string _apiKey; // The API key for authenticating requests.
		private readonly HttpClient _httpClient; // The HTTP client used to make requests.

		/// <summary>
		/// Initializes a new instance of the <see cref="InfuraApiService"/> class with the specified configuration.
		/// </summary>
		/// <param name="configuration">The application configuration containing API URL and key.</param>
		public InfuraApiService(IConfiguration configuration)
		{
			_apiUrl = configuration["ApiUrls:InfuraApi"];
			_apiKey = Environment.GetEnvironmentVariable("INFURA_API_KEY");
			_httpClient = new HttpClient();
		}

		/// <summary>
		/// Asynchronously retrieves the base fee history for a specified block number.
		/// </summary>
		/// <param name="blockNumber">The block number for which to retrieve the base fee history.</param>
		/// <returns>
		/// A task representing the asynchronous operation, containing a list of base fees as decimals.
		/// </returns>
		/// <exception cref="Exception">Thrown when an error occurs while fetching or processing the base fee data.</exception>
		public async Task<List<decimal>> GetBaseFeeHistory(long blockNumber)
		{
			var apiUrl = _apiUrl + _apiKey;
			string response = string.Empty;

			// Convert block number to hexadecimal format
			var hexBlockNumber = "0x" + blockNumber.ToString("X");

			// Create the request body for the JSON-RPC call
			var requestBody = new
			{
				id = 1,
				jsonrpc = "2.0",
				method = "eth_feeHistory",
				@params = new object[] { "0xA", hexBlockNumber, new object[] { } }
			};

			try
			{
				// Serialize the request body to JSON
				var jsonRequestBody = JsonSerializer.Serialize(requestBody);
				var content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json");

				// Send the request to the Infura API
				var httpResponse = await _httpClient.PostAsync(apiUrl, content);
				httpResponse.EnsureSuccessStatusCode();

				// Read the response content
				response = await httpResponse.Content.ReadAsStringAsync();

				// Deserialize the response data
				var responseData = JsonSerializer.Deserialize<JsonRpcResponse>(response);

				if (responseData?.Result == null)
				{
					Logger.Warn("Base fee history is empty or null. Response: {Response}", response);
					return new List<decimal>();
				}

				// Extract base fee per gas from the response
				var baseFeePerGasHex = JsonSerializer.Deserialize<Dictionary<string, object>>(responseData.Result.ToString())["baseFeePerGas"] as JsonElement?;

				if (baseFeePerGasHex == null || !baseFeePerGasHex.Value.ValueKind.Equals(JsonValueKind.Array))
				{
					Logger.Warn("Base fee history is empty or null. Response: {Response}", response);
					return new List<decimal>();
				}

				var baseFeeList = new List<decimal>();
				foreach (var item in baseFeePerGasHex.Value.EnumerateArray())
				{
					var hexValue = item.GetString();
					if (hexValue != null)
					{
						// Remove '0x' prefix and parse the hex value
						var hexWithoutPrefix = hexValue.StartsWith("0x") ? hexValue[2..] : hexValue;
						var bigIntegerValue = long.Parse(hexWithoutPrefix, NumberStyles.AllowHexSpecifier);
						var decimalValue = (decimal)bigIntegerValue / (decimal)Math.Pow(10, 9); // Convert to decimal
						baseFeeList.Add(decimalValue);
					}
				}

				return baseFeeList;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "An error occurred while fetching base fee history.");
				return new List<decimal>();
			}
		}
	}
}

