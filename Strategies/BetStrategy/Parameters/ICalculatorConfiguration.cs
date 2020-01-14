using Core.Components;

namespace Strategies.BetStrategy.Parameters
{
    public interface ICalculatorConfiguration
    {
        IBetMultiplierCalculator ToBetCalculator();
        string Equation { get; }
        BetFunctionType Type { get; }
    }
}
