using Core.Constants;
using Microsoft.AspNetCore.Mvc;
using Strategies;

namespace BlackjackAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly StrategiesResolver _strategiesResolver;

        public ConfigurationController(StrategiesResolver strategiesResolver)
        {
            _strategiesResolver = strategiesResolver;
        }

        [HttpPost]
        [Route("countingStrategy")]
        public IActionResult SetCountingStrategy([FromBody] SetCountingStrategyModel model)
        {
            _strategiesResolver.SetCountingStrategy(model.Strategy);
            return new OkObjectResult(new
            {
                Message = "Ok",
                NewStrategy = _strategiesResolver.CountingStrategy
            });
        }
        public class SetCountingStrategyModel
        {
            public CountingStrategy Strategy { get; set; }
        }

        [HttpGet]
        [Route("countingStrategy")]
        public IActionResult GetCountingStrategy()
        {
            return new OkObjectResult(new
            {
                Strategy = _strategiesResolver.BetFunction
            });
        }

        [HttpPost]
        [Route("betStrategy")]
        public IActionResult SetBetStrategy([FromBody] SetBetStrategyModel model)
        {
            _strategiesResolver.SetMultiplierStrategy(model.Function);
            return new OkObjectResult(new
            {
                Message = "Ok",
                NewStrategy = model.Function
            });
        }
        public class SetBetStrategyModel
        {
            public string Function { get; set; }
        }

        [HttpGet]
        [Route("betStrategy")]
        public IActionResult GetBetStrategy()
        {
            return new OkObjectResult(new
            {
                Strategy = _strategiesResolver.BetFunction
            });
        }
    }
}