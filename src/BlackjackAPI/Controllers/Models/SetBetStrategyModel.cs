using Newtonsoft.Json.Linq;
using Strategies.BetStrategy;
using Strategies.BetStrategy.Parameters;
using System;

namespace BlackjackAPI.Controllers.Models
{

    public class SetBetStrategyModel
    {
        public BetFunctionType FunctionType { get; set; }
        public JObject Parameters { get; set; }
        public ICalculatorConfiguration TryBindCalculatorConfiguration()
        {
            try
            {
                return FunctionType switch
                {
                    BetFunctionType.Linear => Parameters.ToObject<LinearConfiguration>() as ICalculatorConfiguration,
                    BetFunctionType.Quadratic => Parameters.ToObject<QuadraticConfiguration>(),
                    BetFunctionType.Logistic => Parameters.ToObject<LogisticFunctionConfiguration>(),
                    BetFunctionType.AlgebraicSigmoid => Parameters.ToObject<AlgebraicSigmoidConfiguration>(),
                    _ => throw new ArgumentException($"Cannot parse to known bet function parameters, got: {Parameters}")
                };
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Cannot parse to known bet function parameters, got: {Parameters}", e);
            }
        }
    }
}
