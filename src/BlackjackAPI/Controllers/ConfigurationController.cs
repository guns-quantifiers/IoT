using Core.Constants;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Strategies;
using Strategies.BetStrategy;
using Strategies.BetStrategy.Parameters;
using System;

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
                Strategy = _strategiesResolver.CountingStrategy
            });
        }

        [HttpPost]
        [Route("betStrategy")]
        public IActionResult SetBetStrategy([FromBody] SetBetStrategyModel model)
        {
            _strategiesResolver.SetMultiplierStrategy(BindCalculatorConfiguration(model));
            return new OkObjectResult(new
            {
                Message = "Ok",
                NewStrategy = model.FunctionType
            });

            ICalculatorConfiguration BindCalculatorConfiguration(SetBetStrategyModel model)
            {
                try
                {
                    return model.FunctionType switch
                    {
                        BetFunctionType.Linear => model.Parameters.ToObject<LinearConfiguration>() as ICalculatorConfiguration,
                        BetFunctionType.Quadratic => model.Parameters.ToObject<QuadraticConfiguration>(),
                        //BetFunctionType.UnipolarSigmoid => model.Parameters.ToObject<UnipolarSigmoidalBetCalculator>(),
                        _ => throw new ArgumentException($"Cannot parse to known bet function parameters, got: {model.Parameters}")
                    };
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Cannot parse to known bet function parameters, got: {model.Parameters}", e);
                }
            }
        }
        public class SetBetStrategyModel
        {
            public BetFunctionType FunctionType { get; set; }
            public JObject Parameters { get; set; }
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