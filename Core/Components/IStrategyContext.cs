using Core.Models;

namespace Core.Components
{
    public interface IStrategyContext
    {
        int GetCounter(Game game, Deal deal);
        int GetCounter(Game game);
    }
}
