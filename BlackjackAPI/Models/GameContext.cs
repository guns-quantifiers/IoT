using Optional;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class GameContext
    {
        public HashSet<Game> Games { get; set; } = new HashSet<Game>();

        public Option<Success, Error> Add(Game game)
        {
            if (Games.Contains(game))
            {
                return Option.None<Success, Error>(new Error("Game already added."));
            }
            Games.Add(game);
            return Option.Some<Success, Error>(new Success("Game successfully added."));
        }

        public class Success
        {
            public Success(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }
    }
}
