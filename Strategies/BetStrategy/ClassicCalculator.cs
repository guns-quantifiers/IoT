using Core.Components;
using Core.Models;
using Strategies.BetStrategy.Parameters;
using System;

namespace Strategies.BetStrategy
{
    public class ClassicCalculator : IBetMultiplierCalculator
    {
        public BetMultiplier Calculate(double counter)
        {
            if (counter % 1 == 0)
            {
                throw new ArgumentException("Cannot use current type of bet function with floating point counter value.");
            }
            return new BetMultiplier
            {
                Value = counter < 1
                    ? 0
                    : counter == 1
                        ? 1
                        : counter == 2
                            ? 2
                            : counter == 3
                                ? 4
                                : counter == 4
                                    ? 8
                                    : 12 // counter >= 5
            };
        }
    }

    public class ClassicConfiguration : ICalculatorConfiguration
    {
        public IBetMultiplierCalculator ToBetCalculator() => new ClassicCalculator();
        public string Equation => "x<1: 0, x=1: 1, x=2: 2, x=3: 4, x=4: 8, x>=5: 12";
        public BetFunctionType Type => BetFunctionType.Classic;
    }
}
