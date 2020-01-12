using Core.Components;
using Core.Constants;
using Core.Exceptions;
using Core.Models;
using System;

namespace Strategies
{
    public class StrategiesResolver
    {
        private readonly Func<CountingStrategy, IStrategyContext> _countingStrategyFunc;
        private readonly Func<string, IBetMultiplierCalculator> _betMultiplierFunc;
        private string _betMultiplierCalculatorFunction;
        private CountingStrategy _countingStrategy;

        public string BetFunction { get; }
        public CountingStrategy CountingStrategy { get; }

        public StrategiesResolver(
            string betMultiplierCalculatorFunction,
            CountingStrategy countingStrategy,
            Func<CountingStrategy, IStrategyContext> countingStrategyFunc,
            Func<string, IBetMultiplierCalculator> betMultiplierFunc)
        {
            _betMultiplierCalculatorFunction = betMultiplierCalculatorFunction;
            _countingStrategy = countingStrategy;
            _countingStrategyFunc = countingStrategyFunc;
            _betMultiplierFunc = betMultiplierFunc;
        }

        public StrategyInfo GetStrategy(Game game, Deal deal)
        {
            try
            {
                int countingCounter = _countingStrategyFunc(_countingStrategy).GetCounter(game, deal);
                BetMultiplier multiplier = _betMultiplierFunc(_betMultiplierCalculatorFunction).Calculate(countingCounter);
                return new StrategyInfo(countingCounter, multiplier);
            }
            catch (Exception e)
            {
                throw new StrategyException($"Could not get full strategy info for {deal.Id.Value}. Check logs for more information.", e);
            }
        }

        public void SetCountingStrategy(CountingStrategy type)
        {
            _countingStrategy = type;
        }

        public void SetMultiplierStrategy(string function)
        {
            _betMultiplierCalculatorFunction = function;
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
