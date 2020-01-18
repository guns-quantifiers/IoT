using BlackjackAPI.Controllers.Models;
using Core.Constants;
using Microsoft.AspNetCore.Mvc;
using Strategies;
using Strategies.BetStrategy;
using Strategies.BetStrategy.Parameters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            _strategiesResolver.SetCountingStrategy(model);
            return new OkObjectResult(new
            {
                Message = "Ok",
                NewStrategy = _strategiesResolver.CountingStrategyConfiguration
            });
        }

        [HttpGet]
        [Route("countingStrategy")]
        public IActionResult GetCountingStrategy()
        {
            return new OkObjectResult(new
            {
                Strategy = _strategiesResolver.CountingStrategyConfiguration
            });
        }

        [HttpGet]
        [Route("countingStrategy/all")]
        public IActionResult GetAllCountingStrategies()
        {
            return new OkObjectResult(new
            {
                Strategies = typeof(CountingStrategy).GetEnumNames()
            });
        }

        [HttpPost]
        [Route("betStrategy")]
        public IActionResult SetBetStrategy([FromBody] SetBetStrategyModel model)
        {
            _strategiesResolver.SetMultiplierStrategy(model.TryBindCalculatorConfiguration());
            return new OkObjectResult(new
            {
                Message = "Ok",
                NewStrategy = model.FunctionType
            });
        }

        [HttpGet]
        [Route("betStrategy")]
        public IActionResult GetBetStrategy()
        {
            return new OkObjectResult(new
            {
                Strategy = new
                {
                    Name = _strategiesResolver.BetFunctionType,
                    Formula = _strategiesResolver.BetFunctionEquation,
                    Params = GetWritableProperties(_strategiesResolver.BetCalculatorConfiguration)
                }
            });

            ICollection GetWritableProperties(ICalculatorConfiguration value) =>
                value.GetType().GetProperties().Where(p => p.CanWrite).Select(p => new
                {
                    Name = p.Name,
                    Value = p.GetValue(value)
                }).ToList();
        }

        [HttpGet]
        [Route("betStrategy/all")]
        public IActionResult GetAllBetStrategies()
        {
            return new OkObjectResult(new
            {
                Types = new List<BetFunctionConfigurationResponse>
                {
                    new BetFunctionConfigurationResponse(BetFunctionType.Linear,
                        GetWritablePropertiesNames<LinearConfiguration>()),
                    new BetFunctionConfigurationResponse(BetFunctionType.Classic,
                        GetWritablePropertiesNames<ClassicConfiguration>()),
                    new BetFunctionConfigurationResponse(BetFunctionType.Quadratic,
                        GetWritablePropertiesNames<QuadraticConfiguration>()),
                    new BetFunctionConfigurationResponse(BetFunctionType.Logistic,
                        GetWritablePropertiesNames<LogisticFunctionConfiguration>()),
                    new BetFunctionConfigurationResponse(BetFunctionType.AlgebraicSigmoid,
                        GetWritablePropertiesNames<AlgebraicSigmoidConfiguration>()),
                }
            });

            List<string> GetWritablePropertiesNames<T>() =>
                typeof(T).GetProperties().Where(p => p.CanWrite).Select(p => p.Name).ToList();
        }

        private class BetFunctionConfigurationResponse
        {
            public BetFunctionConfigurationResponse(BetFunctionType name, List<string> parameters)
            {
                Name = name;
                Parameters = parameters;
            }

            public BetFunctionType Name { get; }
            public List<string> Parameters { get; }
        }
    }
}