using Core.Components;
using Core.Models;
using Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlackjackAPI.Services
{
    public class GameSaver : IGameSaver
    {
        private readonly IOptionsMonitor<PersistenceSettings> _persistenceSettingsOptions;

        public GameSaver(IOptionsMonitor<PersistenceSettings> persistenceSettingsOptions)
        {
            _persistenceSettingsOptions = persistenceSettingsOptions;
        }

        public void SaveGames(List<Game> games) => File.WriteAllText(_persistenceSettingsOptions.CurrentValue.FilePath, JsonConvert.SerializeObject(games, Formatting.Indented));

        public List<Game> LoadGames()
        {
            try
            {
                List<Game> games = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(_persistenceSettingsOptions.CurrentValue.FilePath));
                return games ?? new List<Game>();
            }
            catch (Exception)
            {
                return new List<Game>();
            }
        }
    }
}