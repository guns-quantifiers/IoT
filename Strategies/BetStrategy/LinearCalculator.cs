using Core.Components;
using Core.Models;
using Strategies.BetStrategy.Parameters;

namespace Strategies.BetStrategy
{
    public class LinearCalculator : IBetMultiplierCalculator
    {
        private readonly double _a;
        private readonly double _b;

        public LinearCalculator(double a, double b)
        {
            _a = a;
            _b = b;
        }

        public BetMultiplier Calculate(double counter) => new BetMultiplier
        {
            Value = _a * counter + _b
        };
    }

    public class LinearConfiguration : ICalculatorConfiguration
    {
        public double A { get; set; }
        public double B { get; set; }

        public IBetMultiplierCalculator ToBetCalculator()
        {
            return new LinearCalculator(A, B);
        }

        public string Equation => $"{A.ToString("F2")}x + {B.ToString("F2")}";
        public BetFunctionType Type => BetFunctionType.Linear;
    }
}
