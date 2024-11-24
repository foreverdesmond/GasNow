using Microsoft.AspNetCore.Mvc;
using GasNow.Business;
using GasNow.Dto;
using NLog;
using StackExchange.Redis;

namespace GasNow.Api.Controllers
{
    /// <summary>
    /// Provides API endpoints for retrieving price information of cryptocurrencies.
    /// </summary>
    [ApiController]
    [Route("api/price")]
    public class PriceController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly PriceBussiness _priceBusiness; // Business logic for price operations.

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceController"/> class.
        /// </summary>
        /// <param name="businessFactory">Factory for creating business logic instances.</param>
        /// <param name="redis">The Redis connection multiplexer.</param>
        public PriceController(IBusinessFactory businessFactory, IConnectionMultiplexer redis)
        {
            _priceBusiness = businessFactory.Create<PriceBussiness>(redis);
        }

        /// <summary>
        /// Asynchronously retrieves the current price of cryptocurrencies.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{GasFeeDto}"/> containing the current price data.
        /// If no data is available, returns a 404 Not Found response.
        /// </returns>
        [HttpGet("currentprice")]
        public async Task<ActionResult<GasFeeDto>> GetCurrentPrice()
        {
            var price = await _priceBusiness.GetCurrentPrice();

            if (price == null)
            {
                Logger.Error("Current Price is Null.");
                return NotFound("No price data available.");
            }

            return Ok(price);
        }
    }
}
