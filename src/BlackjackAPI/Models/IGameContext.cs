using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public interface IGameContext
    {
        IReadOnlyDictionary<Guid, Game> Games { get; }

        Game NewGame();
        void Initialize(bool shouldUseTestData = true);
        void Add(Game game);
        void Add(Guid gameId, Deal deal);
        void ClearAll();
        void Save();
    }
}