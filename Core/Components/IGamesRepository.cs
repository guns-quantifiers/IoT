using Core.Models;
using System.Collections.Generic;

namespace Core.Components
{
    public interface IGamesRepository
    {
        IReadOnlyDictionary<GameId, Game> Games { get; }

        Game NewGame();
        void Initialize(bool shouldUseTestData = true);
        void ClearAll();
        void Save();
    }
}