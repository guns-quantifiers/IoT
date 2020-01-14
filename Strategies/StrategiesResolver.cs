using Core.Components;
using Core.Constants;
using Core.Exceptions;
using Core.Models;
using Strategies.BetStrategy;
using Strategies.BetStrategy.Parameters;
using System;

namespace Strategies
{
    public class StrategiesResolver
    {
        private readonly Func<CountingStrategy, IStrategyContext> _countingStrategyFunc;
        private ICalculatorConfiguration _betMultiplierCalculatorConfiguration = new LinearConfiguration { A = 1, B = 0 };

        public string BetFunctionEquation => _betMultiplierCalculatorConfiguration.Equation;
        public BetFunctionType BetFunctionType => _betMultiplierCalculatorConfiguration.Type;
        public ICalculatorConfiguration BetCalculatorConfiguration => _betMultiplierCalculatorConfiguration;
        public CountingStrategy CountingStrategy { get; private set; }

        public StrategiesResolver(
            CountingStrategy countingStrategy,
            Func<CountingStrategy, IStrategyContext> countingStrategyFunc)
        {
            CountingStrategy = countingStrategy;
            _countingStrategyFunc = countingStrategyFunc;
        }

        public StrategyInfo GetStrategy(Game game, Deal deal)
        {
            try
            {
                int countingCounter = _countingStrategyFunc(CountingStrategy).GetCounter(game, deal);
                BetMultiplier multiplier = _betMultiplierCalculatorConfiguration.ToBetCalculator().Calculate(countingCounter);
                return new StrategyInfo(countingCounter, multiplier);
            }
            catch (Exception e)
            {
                throw new StrategyException($"Could not get full strategy info for {deal.Id.Value}. Check logs for more information.", e);
            }
        }

        public void SetCountingStrategy(CountingStrategy type)
        {
            CountingStrategy = type;
        }

        public void SetMultiplierStrategy(ICalculatorConfiguration configuration)
        {
            _betMultiplierCalculatorConfiguration = configuration;
        }
    }
}

public class StrategyInfo
{
    public StrategyInfo(int counter, BetMultiplier betMultiplier)
    {
        Counter = counter;
        BetMultiplier = betMultiplier;
    }

    public int Counter { get; }
    public BetMultiplier BetMultiplier { get; }
}
