using Core.Components;
using Core.Models;
using Strategies.BetStrategy.Parameters;
using System;

namespace Strategies.BetStrategy
{
    public class LogisticFunctionBetCalculator : IBetMultiplierCalculator
    {
        private readonly double _l;
        private readonly double _k;
        private readonly double _x0;

        public LogisticFunctionBetCalculator(double l, double k, double x0)
        {
            _l = l;
            _k = k;
            _x0 = x0;
        }

        public BetMultiplier Calculate(int counter) => MultiplierFunction(counter);

        private BetMultiplier MultiplierFunction(int counter)
            => new BetMultiplier
            {
                Value = _l / (1 + Math.Exp(-_k * (counter - _x0)))
            };
    }

    public class LogisticFunctionConfiguration : ICalculatorConfiguration
    {
        public double L { get; set; }
        public double K { get; set; }
        public double X0 { get; set; }

        public IBetMultiplierCalculator ToBetCalculator()
        {
            return new LogisticFunctionBetCalculator(L, K, X0);
        }

        public string Equation => $"{L:F2} / (1 + e^(-{K:F2}(x-{X0:F2})))";
        public BetFunctionType Type => BetFunctionType.Logistic;
    }
}
