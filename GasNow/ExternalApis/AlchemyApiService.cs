using System;
using GasNow.Helper;
using Microsoft.Extensions.Configuration;
using NLog;

namespace GasNow.ExternalApis
{
	public class AlchemyApiService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly string _apiUrl; // The base URL for the Alchemy API.
		private readonly string _apiKey; // The API key for authenticating requests.
		private readonly HttpClient _httpClient; // The HTTP client used to make requests.

		/// <summary>
		/// Initializes a new instance of the <see cref="AlchemyApiService"/> class with the specified configuration.
		/// </summary>
		/// <param name="configuration">The application configuration containing API URL and key.</param>
		public AlchemyApiService(IConfiguration configuration)
		{
			_apiUrl = configuration["ApiUrls:AlchemyApi"];
			_apiKey = Environment.GetEnvironmentVariable("BLOCKNAVIE_API_KEY");
			_httpClient = new HttpClient();
		}
	}
}

