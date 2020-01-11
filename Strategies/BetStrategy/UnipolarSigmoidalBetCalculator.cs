using Core.Components;
using Core.Models;
using System;

namespace Strategies.BetStrategy
{
    public class UnipolarSigmoidalBetCalculator : IBetMultiplierCalculator
    {
        private readonly int _maxBetMultiplier = 10;
        private readonly double _counterMultiplier = 1;

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
