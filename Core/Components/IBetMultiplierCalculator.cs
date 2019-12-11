using Core.Models;

namespace Core.Components
{
    public interface IBetMultiplierCalculator
    {
        BetMultiplier Calculate(int counter);
    }
}