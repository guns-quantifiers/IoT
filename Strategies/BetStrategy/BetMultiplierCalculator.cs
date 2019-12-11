using Core.Components;
using Core.Models;
using System;

namespace Strategies.BetStrategy
{
    public class BetMultiplierCalculator : IBetMultiplierCalculator
    {
        public BetMultiplier Calculate(int counter) => MultiplierFunction(counter);

        private BetMultiplier MultiplierFunction(int counter) 
            => new BetMultiplier
            {
                Value = 2.0 / (1 + Math.Exp(-0.098 * counter))
            };
    }
}
