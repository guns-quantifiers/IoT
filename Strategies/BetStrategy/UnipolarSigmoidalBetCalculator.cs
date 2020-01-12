using Core.Components;
using Core.Models;
using System;

namespace Strategies.BetStrategy
{
    public class UnipolarSigmoidalBetCalculator : IBetMultiplierCalculator
    {
        private readonly int _maxBetMultiplier;
        private readonly double _counterMultiplier;

        public UnipolarSigmoidalBetCalculator(int maxBetMultiplier = 10, double counterMultiplier = 1)
        {
            _maxBetMultiplier = maxBetMultiplier;
            _counterMultiplier = counterMultiplier;
        }

        public BetMultiplier Calculate(int counter) => MultiplierFunction(counter);

        private BetMultiplier MultiplierFunction(int counter)
            => new BetMultiplier
            {
                Value = counter > 0
                    ? 2.0 / (1 + Math.Exp(-_counterMultiplier * counter))
                    : 1
            };
    }
}
