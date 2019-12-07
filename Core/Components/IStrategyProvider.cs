using Core.Constants;
using Core.Models;

namespace Core.Components
{
    public interface IStrategyProvider
    {
        DrawStrategy Get(Game game, Deal deal);
    }
}
