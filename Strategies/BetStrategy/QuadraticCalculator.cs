using Core.Components;
using Core.Models;
using Strategies.BetStrategy.Parameters;

namespace Strategies.BetStrategy
{
    public class QuadraticCalculator : IBetMultiplierCalculator
    {
        private readonly double _a;
        private readonly double _b;
        private readonly double _c;

        public QuadraticCalculator(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public BetMultiplier Calculate(double counter)
        {
            if (counter <= 0)
            {
                return new BetMultiplier
                {
                    Value = 0
                };
            }
            return new BetMultiplier
            {
                Value = _a * counter * counter + _b * counter + _c
            };
        }
    }

    public class QuadraticConfiguration : ICalculatorConfiguration
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public IBetMultiplierCalculator ToBetCalculator()
        {
            return new QuadraticCalculator(A, B, C);
        }

        public string Equation => $"{A.ToString("F2")}x^2 + {B.ToString("F2")}x + {C.ToString("F2")}";
        public BetFunctionType Type => BetFunctionType.Quadratic;
    }
}
