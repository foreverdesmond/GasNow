using GasNow.Business;
using GasNow.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GasNow.Service;
using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using DotNetEnv;

namespace GasNow
{ 
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static IConfiguration _configuration { get; private set; }

        public static async Task Main(string[] args)
        {
            Env.Load();

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    //var context = services.GetRequiredService<GasNowDbContext>();
                    var redis = services.GetRequiredService<IConnectionMultiplexer>();
                    var businessFactory = services.GetRequiredService<IBusinessFactory>();
                    //var gasFeeBusiness = businessFactory.Create<GasFeeBusiness>(context, redis);
                    //var gasFeeBusiness = businessFactory.Create<GasFeeBusiness>(redis,_configuration);
                    //var chainApiUrlBusiness = businessFactory.Create<ChainApiUrlBusiness>(context);

                    //var priceBusiness = businessFactory.Create<PriceBussiness>(context, redis);

                    var priceBusiness = businessFactory.Create<PriceBussiness>(redis,_configuration);

                    //var taskGasFee = StartFetchingGasFeesAsync(gasFeeBusiness);

                    var gasFeeBlockNavieBussiness = businessFactory.Create<GasFeeBlockNavieBusiness>(redis, _configuration);

                    var taskGasFeeBlockNavie = StartFetchingGasFeeBlockNavieAsynce(gasFeeBlockNavieBussiness);

                    var taskPrice = StartFetchingPriceAsync(priceBusiness);

                    await Task.WhenAll(taskGasFeeBlockNavie, taskPrice);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "An error occurred while fetching gas fees.");
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logging.AddNLog();
                })
            .ConfigureServices((hostContext, services) =>
            {
                _configuration = hostContext.Configuration;

                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<GasNowDbContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(_configuration.GetConnectionString("RedisConnection")));

                services.AddScoped<ChainAPIUrlService>();
                services.AddScoped<ChainService>();
                services.AddScoped<GasFeeService>();
                services.AddScoped<ChainApiUrlBusiness>();
                services.AddScoped<GasFeeBusiness>();
                services.AddScoped<PriceBussiness>();

                services.AddSingleton<IBusinessFactory, BusinessFactory>();
                services.AddControllers();
            });
           
        public static async Task StartFetchingGasFeesAsync(GasFeeBusiness gasFeeBusiness)
        {
            using var httpClient = new HttpClient();

            var useDatabase = Convert.ToBoolean(_configuration["DatabaseSettings:UseDatabase"]);

            //var apiUrl = chainApiUrlBusiness.GetApiUrlByName("GasNow",useDatabase);

            //var apiKey = _configuration["ApiUrls:EtherscanApiKey"];

            //apiUrl = apiUrl + apiKey;

            //if (!string.IsNullOrEmpty(apiUrl))
            //{
            while (true)
            {
                try
                {
                    await gasFeeBusiness.GetAndSaveGasFeesAsync(useDatabase);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "An error occurred while fetching gas fees.");
                }

                await Task.Delay(120000);
            }
            //}
            //else
            //{
            //    Logger.Error("Read ApiUrl Error. Plaease check conifg in DB or appsetting.");
            //}
        }

        public static async Task StartFetchingPriceAsync(PriceBussiness priceBussiness)
        {
            using var httpClient = new HttpClient();

            var useDatabase = Convert.ToBoolean(_configuration["DatabaseSettings:UseDatabase"]);

            //var apiUrl = chainApiUrlBusiness.GetApiUrlByName("PriceNow",useDatabase);

            //var apiKey = _configuration["ApiUrls:EtherscanApiKey"];

            //apiUrl = apiUrl + apiKey;

            //if (!string.IsNullOrEmpty(apiUrl))
            //{
            while (true)
            {
                try
                {
                    await priceBussiness.GetAndSavePriceAsync(_configuration, useDatabase);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "An error occurred while fetching price.");
                }

                await Task.Delay(120000);
            }
            //}
            //else
            //{
            //    Logger.Error("Read ApiUrl Error. Plaease check conifg in DB or appsetting.");
            //}
        }

        public static async Task StartFetchingGasFeeBlockNavieAsynce(GasFeeBlockNavieBusiness gasFeeBlockNavieBusiness)
        {
            using var httpClient = new HttpClient();

            while (true)
            {
                try
                {
                    //ETH 
                    await gasFeeBlockNavieBusiness.GetAndSaveGasFeesAsync(1);
                    await Task.Delay(5000);
                    //Arbitrum One
                    await gasFeeBlockNavieBusiness.GetAndSaveGasFeesAsync(42161);
                    await Task.Delay(5000);
                    //Base
                    await gasFeeBlockNavieBusiness.GetAndSaveGasFeesAsync(8453);
                    await Task.Delay(5000);
                    //Linea
                    await gasFeeBlockNavieBusiness.GetAndSaveGasFeesAsync(59144);
                    await Task.Delay(5000);
                    //Optimism
                    await gasFeeBlockNavieBusiness.GetAndSaveGasFeesAsync(10);
                    await Task.Delay(5000);
                    //ZKsync
                    await gasFeeBlockNavieBusiness.GetAndSaveGasFeesAsync(324);
                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "An error occurred while fetching gas fees.");
                }

                //await Task.Delay(6000);
            }
        }
    }
}