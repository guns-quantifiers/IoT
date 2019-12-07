using System;
using Core.Models;

namespace Core.Components
{
    public interface IBetMultiplierCalculator
    {
        BetMultiplier Calculate(Game game, Guid dealId);
    }
}