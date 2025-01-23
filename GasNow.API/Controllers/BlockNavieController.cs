using Microsoft.AspNetCore.Mvc;
using GasNow.Business;
using GasNow.Dto;
using NLog;
using StackExchange.Redis;


namespace GasNowAPI.Controllers
{
    [ApiController]
    [Route("api/GasFeeBlockNavie")]
    public class BlockNavieController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly GasFeeBlockNavieBusiness _gasFeeBlockNavieBusiness; // Business logic for gas fee operations.

        public BlockNavieController(IBusinessFactory businessFactory, IConnectionMultiplexer redis, IConfiguration configuration)
		{
            _gasFeeBlockNavieBusiness = businessFactory.Create<GasFeeBlockNavieBusiness>(redis, configuration);
        }

        [HttpGet("currentgas")]
        public async Task<ActionResult<GasFeeBlockNavieDto>> GetCurrentGasFee(string networkId)
        {
            var gasFee = await _gasFeeBlockNavieBusiness.GetCurrentGasFee(networkId);

            if (gasFee == null)
            {
                Logger.Error("Current Gas Fee is Null.");
                return NotFound("No gas fee data available.");
            }

            return Ok(gasFee);
        }
    }
}

