using Microsoft.AspNetCore.Mvc;
using GasNow.Business;
using GasNow.Dto;
using NLog;
using StackExchange.Redis;

namespace GasNow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly PriceBussiness _priceBusiness;

        public PriceController(IConnectionMultiplexer redis, IBusinessFactory businessFactory)
        {
            _priceBusiness = businessFactory.Create<PriceBussiness>(redis);
        }

        [HttpGet("current")]
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
