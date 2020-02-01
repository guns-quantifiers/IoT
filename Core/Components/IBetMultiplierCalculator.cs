using Core.Models;

namespace Core.Components
{
    public interface IBetMultiplierCalculator
    {
        BetMultiplier Calculate(double counter);
    }
}