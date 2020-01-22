using Core.Models;
using System.Collections.Generic;

namespace Core.Components
{
    public interface IGamesRepository
    {
        IReadOnlyDictionary<GameId, Game> Games { get; }

        bool TryFindOne(GameId id, out Game game);
        bool TryFindGameForDeal(DealId id, out Game game);
        Game NewGame();
        void Update(Game game);
        void ClearAll();
        IEnumerable<Game> GetAll();
    }
}