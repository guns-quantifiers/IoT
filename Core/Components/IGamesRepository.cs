using Core.Models;
using System.Collections.Generic;

namespace Core.Components
{
    public interface IGamesRepository
    {
        IReadOnlyDictionary<GameId, Game> Games { get; }

        Game NewGame();
        void Update(Game game);
        void ClearAll();
    }
}