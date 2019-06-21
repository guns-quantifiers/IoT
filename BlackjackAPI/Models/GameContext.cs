using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class GameContext
    {
        public Dictionary<Guid, Game> Games { get; set; } = new Dictionary<Guid, Game>();

        public void Add(Game game)
        {
            if (Games.ContainsKey(game.Id))
            {
                throw new ApplicationException("Game already added.");
            }
            Games.Add(game.Id, game);
        }
    }
}
