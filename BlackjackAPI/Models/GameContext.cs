using BlackjackAPI.Exceptions;
using BlackjackAPI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackAPI.Models
{
    public class GameContext
    {
        private readonly GameSaver _saver;
        private readonly ILogger<GameContext> _logger;

        public GameContext(GameSaver saver, ILogger<GameContext> logger)
        {
            _saver = saver;
            _logger = logger;

            Initialize();
        }

        private Dictionary<Guid, Game> _games = new Dictionary<Guid, Game>();

        public IReadOnlyDictionary<Guid, Game> Games => _games;

        public void Add(Game game)
        {
            if (_games.ContainsKey(game.Id))
            {
                throw new DuplicateKeyException("Game already added.");
            }
            _games.Add(game.Id, game);
            _saver.SaveGames(_games.Values.ToList());
        }

        public void Add(Guid gameId, Deal deal)
        {
            if(_games.TryGetValue(gameId, out Game currentGame))
            {
                currentGame.History.Add(deal);
                _saver.SaveGames(_games.Values.ToList());
            }
            else
            {
                throw new NotFoundException($"Game not found: {gameId}.");
            }
        }

        public void Save()
        {
            _saver.SaveGames(_games.Values.ToList());
        }

        private void Initialize()
        {
            var savedGames = _saver.LoadGames();
            if (savedGames.Any())
            {
                if (savedGames.GroupBy(g => g.Id).Any(g => g.Count() > 1))
                {
                    _logger.LogWarning("Games read from file contained duplicated keys that were ignored.");
                }
                savedGames.GroupBy(g => g.Id)
                    .Select(g => g.First())
                    .ToList()
                    .ForEach(g =>
                    {
                        _games.Add(g.Id, g);
                    });
            }
            else
            {
                AddTestData();
            }
        }

        private void AddTestData()
        {
            var startDeals = new List<Deal>()
            {
                new Deal()
                {
                    Id = Guid.NewGuid(),
                    CroupierHand = new List<CardType>{CardType.Queen},
                    PlayerHand = new List<CardType>{CardType.Five, CardType.Jack}
                },
                new Deal()
                {
                    Id = Guid.NewGuid(),
                    CroupierHand = new List<CardType>{CardType.Ace},
                    PlayerHand = new List<CardType>{CardType.Two, CardType.Six}
                }
            };
            var game = new Game()
            {
                Id = Guid.NewGuid(),
                History = startDeals
            };
            _games.Add(
                game.Id,
                game
            );
        }
    }
}