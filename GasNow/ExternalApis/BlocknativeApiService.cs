using System;
using GasNow.Helper;
using Microsoft.Extensions.Configuration;
using NLog;
using GasNow.Module;
using DotNetEnv;
using GasNow.Dto;
using Newtonsoft.Json;

namespace GasNow.ExternalApis
{
    public class BlocknativeApiService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _apiUrl; // The base URL for the Alchemy API.
        private readonly string _apiKey; // The API key for authenticating requests.
        private readonly HttpClient _httpClient; // The HTTP client used to make requests.
        private readonly string _networkId;

        public BlocknativeApiService(IConfiguration configuration,int networkId)
        {
            _apiUrl = configuration["ApiUrls:BlockNavieApi"];
            _apiKey = Environment.GetEnvironmentVariable("BLOCKNAVIE_API_KEY");
            _httpClient = new HttpClient();
            _networkId = networkId.ToString();
        }

        public async Task<GasFeeBlockNavieDto> GetCurrentGasFeeAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,_apiUrl + _networkId);
            request.Headers.Add("Authorization", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var gasFeeData = JsonConvert.DeserializeObject<GasFeeBlockNavieDto>(jsonResponse);

            return gasFeeData;
        }
    }
}

