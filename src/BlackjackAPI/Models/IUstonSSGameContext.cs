using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public interface IUstonSSGameContext
    {
        IReadOnlyDictionary<Guid, Game> Games { get; }

        void Add(Game game);
        void Add(Guid gameId, Deal deal);
        void ClearAll();
        Game NewGame();
        void Save();
    }
}