using Microsoft.AspNetCore.Mvc;
using GasNow.Business;
using GasNow.Dto;
using NLog;
using StackExchange.Redis;

namespace GasNow.Api.Controllers
{
    /// <summary>
    /// Provides API endpoints for retrieving gas fee information.
    /// </summary>
    [ApiController]
    [Route("api/gasfee")]
    public class GasFeeController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
                
        private readonly GasFeeBusiness _gasFeeBusiness; // Business logic for gas fee operations.

        /// <summary>
        /// Initializes a new instance of the <see cref="GasFeeController"/> class.
        /// </summary>
        /// <param name="businessFactory">Factory for creating business logic instances.</param>
        /// <param name="redis">The Redis connection multiplexer.</param>
        /// <param name="configuration">The application configuration.</param>
        public GasFeeController(IBusinessFactory businessFactory, IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _gasFeeBusiness = businessFactory.Create<GasFeeBusiness>(redis, configuration);
        }

        /// <summary>
        /// Retrieves the current gas fee.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{GasFeeDto}"/> containing the current gas fee data.
        /// If no data is available, returns a 404 Not Found response.
        /// </returns>
        [HttpGet("currentgas")]
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

        /// <summary>
        /// Retrieves the gas fee data for the last week.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{List{decimal}}"/> containing a list of gas fees for the last week.
        /// If no data is available, returns a 404 Not Found response.
        /// </returns>
        [HttpGet("lastweekgas")]
        public async Task<ActionResult<List<decimal>>> GetGasFeeLastWeek()
        {
            var gasFeeLastWeek = await _gasFeeBusiness.GetGasFeeLastWeek();

            if (gasFeeLastWeek == null || gasFeeLastWeek.Count == 0)
            {
                Logger.Error("Last Gas Fee is Null.");
                return NotFound("No gas fee data available.");
            }

            return Ok(gasFeeLastWeek);
        }
    }
}