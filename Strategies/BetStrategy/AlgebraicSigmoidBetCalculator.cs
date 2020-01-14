using Core.Components;
using Core.Models;
using Strategies.BetStrategy.Parameters;
using System;

namespace Strategies.BetStrategy
{
    public class AlgebraicSigmoidBetCalculator : IBetMultiplierCalculator
    {
        private readonly double _l;
        private readonly double _k;
        private readonly double _x0;

        public AlgebraicSigmoidBetCalculator(double l, double k, double x0)
        {
            _l = l;
            _k = k;
            _x0 = x0;
        }

        public BetMultiplier Calculate(int counter) => MultiplierFunction(counter);

        private BetMultiplier MultiplierFunction(int counter)
            => new BetMultiplier
            {
                Value = counter > 0
                    ? _l * counter / (1 + Math.Sqrt(1 + _k * Math.Pow((counter - _x0), 2)))   // l*x / sqrt (1 + k(x-x0)^2)
                    : 1
            };
    }

    public class AlgebraicSigmoidConfiguration : ICalculatorConfiguration
    {
        public double L { get; set; }
        public double K { get; set; }
        public double X0 { get; set; }

        public IBetMultiplierCalculator ToBetCalculator()
        {
            return new AlgebraicSigmoidBetCalculator(L, K, X0);
        }

        public string Equation => $"{L:F2}x / sqrt(1 + {K:F2}(x-{X0:F2}))";
        public BetFunctionType Type => BetFunctionType.AlgebraicSigmoid;
    }
}
