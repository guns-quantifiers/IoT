using Core.Components;
using Core.Constants;
using Core.Exceptions;
using Core.Models;
using Strategies.BetStrategy;
using Strategies.BetStrategy.Parameters;
using Strategies.StrategyContexts.Knockout;
using Strategies.StrategyContexts.SilverFox;
using Strategies.StrategyContexts.UstonSS;
using System;

namespace Strategies
{
    public class StrategiesResolver
    {
        private ICalculatorConfiguration _betMultiplierCalculatorConfiguration = new LinearConfiguration { A = 1, B = 0 };

        public string BetFunctionEquation => _betMultiplierCalculatorConfiguration.Equation;
        public BetFunctionType BetFunctionType => _betMultiplierCalculatorConfiguration.Type;
        public ICalculatorConfiguration BetCalculatorConfiguration => _betMultiplierCalculatorConfiguration;
        public SetCountingStrategyModel CountingStrategyConfiguration { get; private set; }

        public StrategiesResolver(
            SetCountingStrategyModel countingStrategy)
        {
            CountingStrategyConfiguration = countingStrategy;
        }

        public StrategyInfo GetStrategy(Game game, Deal deal)
        {
            try
            {
                int countingCounter = CountingStrategyConfiguration.GetStrategyContext().GetCounter(game, deal);
                BetMultiplier multiplier = _betMultiplierCalculatorConfiguration.ToBetCalculator().Calculate(countingCounter);
                return new StrategyInfo(countingCounter, multiplier);
            }
            catch (Exception e)
            {
                throw new StrategyException($"Could not get full strategy info for {deal.Id.Value}. Check logs for more information.", e);
            }
        }

        public void SetCountingStrategy(SetCountingStrategyModel countingStrategyParameters)
        {
            CountingStrategyConfiguration = countingStrategyParameters;
        }

        public void SetMultiplierStrategy(ICalculatorConfiguration configuration)
        {
            _betMultiplierCalculatorConfiguration = configuration;
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

    public class SetCountingStrategyModel
    {
        public CountingStrategy Strategy { get; set; }
        public bool UseTrueCounter { get; set; }
        public int DeckAmount { get; set; }
        public IStrategyContext GetStrategyContext()
        {
            return Strategy switch
            {
                CountingStrategy.SilverFox => new SilverFoxStrategyContext(DeckAmount, UseTrueCounter) as IStrategyContext,
                CountingStrategy.UstonSS => new UstonSSStrategyContext(DeckAmount, UseTrueCounter),
                CountingStrategy.Knockout => new KnockoutStrategyContext(DeckAmount, UseTrueCounter),
                _ => throw new ArgumentException("Unknown counting strategy: " + Strategy)
            };
        }
    }
}