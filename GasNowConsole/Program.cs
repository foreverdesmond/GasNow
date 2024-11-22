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

namespace GasNow
{ 
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static IConfiguration Configuration { get; private set; }

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<GasNowDbContext>();
                    var redis = services.GetRequiredService<IConnectionMultiplexer>();
                    var businessFactory = services.GetRequiredService<IBusinessFactory>();
                    var gasFeeBusiness = businessFactory.Create<GasFeeBusiness>(context, redis);
                    var chainApiUrlBusiness = businessFactory.Create<ChainApiUrlBusiness>(context);

                    var priceBusiness = businessFactory.Create<PriceBussiness>(context, redis);

                    var taskGasFee = StartFetchingGasFeesAsync(gasFeeBusiness,chainApiUrlBusiness);

                    var taskPrice = StartFetchingPriceAsync(priceBusiness, chainApiUrlBusiness);

                    await Task.WhenAll(taskGasFee, taskPrice);

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
                Configuration = hostContext.Configuration;

                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<GasNowDbContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection")));

                services.AddScoped<ChainAPIUrlService>();
                services.AddScoped<ChainService>();
                services.AddScoped<GasFeeService>();
                services.AddScoped<ChainApiUrlBusiness>();
                services.AddScoped<GasFeeBusiness>();
                services.AddScoped<PriceBussiness>();

                services.AddSingleton<IBusinessFactory, BusinessFactory>();
                services.AddControllers();
            });
           
        public static async Task StartFetchingGasFeesAsync(GasFeeBusiness gasFeeBusiness, ChainApiUrlBusiness chainApiUrlBusiness)
        {
            using var httpClient = new HttpClient();
            var apiUrl = chainApiUrlBusiness.GetApiUrlByName("GasNow");

            var apiKey = Configuration["ApiSettings:EtherscanApiKey"];

            apiUrl = apiUrl + apiKey;
            if (!string.IsNullOrEmpty(apiUrl))
            {
                while (true)
                {
                    try
                    {
                        await gasFeeBusiness.GetAndSaveGasFeesAsync(httpClient, apiUrl);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "An error occurred while fetching gas fees.");
                    }

                    await Task.Delay(6000);
                }
            }
            else
            {
                Logger.Error("Api Url is null. Please check api_url data in DB.");
            }
        }

        public static async Task StartFetchingPriceAsync(PriceBussiness priceBussiness, ChainApiUrlBusiness chainApiUrlBusiness)
        {
            using var httpClient = new HttpClient();
            var apiUrl = chainApiUrlBusiness.GetApiUrlByName("PriceNow");

            var apiKey = Configuration["ApiSettings:EtherscanApiKey"];

            apiUrl = apiUrl + apiKey;

            if (!string.IsNullOrEmpty(apiUrl))
            {
                while (true)
                {
                    try
                    {
                        await priceBussiness.GetPrice(httpClient, apiUrl);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "An error occurred while fetching gas fees.");
                    }

                    await Task.Delay(12000);
                }
            }
            else
            {
                Logger.Error("Api Url is null. Please check api_url data in DB.");
            }
        }
    }
}