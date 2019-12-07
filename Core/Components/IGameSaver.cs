using System.Collections.Generic;
using Core.Models;

namespace Core.Components
{
    public interface IGameSaver
    {
        List<Game> LoadGames();
        void SaveGames(List<Game> games);
    }
}