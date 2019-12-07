using Core.Models;
using System;
using System.Collections.Generic;

namespace Core.Components
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