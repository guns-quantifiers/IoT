﻿using Core.Components;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlackjackAPI.Services
{
    public class GameSaver : IGameSaver
    {
        private readonly IConfiguration _configuration;

        public GameSaver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SaveGames(List<Game> games)
        {
            var filePath = "";// _configuration.GetValue<string>("SaveFilePath");
            //var filePath = _configuration.GetValue<string>("SaveFilePath");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(games, Formatting.Indented));
        }

        public List<Game> LoadGames()
        {
            var filePath = "";// _configuration.GetValue<string>("SaveFilePath");
            
            try
            {
                List<Game> games = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(filePath));
                return games ?? new List<Game>();
            }
            catch (Exception)
            {
                return new List<Game>();
            }
        }
    }
}