using Microsoft.AspNetCore.Mvc;
using GasNow.Business;
using GasNow.Dto;
using NLog;
using StackExchange.Redis;

namespace GasNow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GasFeeController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly GasFeeBusiness _gasFeeBusiness;

        public GasFeeController(IConnectionMultiplexer redis, IBusinessFactory businessFactory)
        {
            _gasFeeBusiness = businessFactory.Create<GasFeeBusiness>(redis);
        }

        [HttpGet("current")]
        public async Task<ActionResult<GasFeeDto>> GetCurrentGasFee()
        {
            var gasFee = await _gasFeeBusiness.GetCurrentGasFee();

            if (gasFee == null)
            {
                Logger.Error("Current Gas Fee is Null.");
                return NotFound("No gas fee data available.");
            }

            return Ok(gasFee);
        }
    }
}