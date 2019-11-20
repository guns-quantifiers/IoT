using System.Collections.Generic;
using BlackjackAPI.Models;

namespace BlackjackAPI.Services
{
    public interface IGameSaver
    {
        List<Game> LoadGames();
        void SaveGames(List<Game> games);
    }
}