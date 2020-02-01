using Core.Models;

namespace Core.Components
{
    public interface IStrategyContext
    {
        double GetCounter(Game game, Deal deal);
    }
}
