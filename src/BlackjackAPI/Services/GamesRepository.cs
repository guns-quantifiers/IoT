using System;
using System.Collections.Generic;
using System.Linq;
using Core.Components;
using Core.Constants;
using Core.Exceptions;
using Core.Models;

namespace BlackjackAPI.Services
{
    public class GamesRepository : IGamesRepository
    {
        private readonly IGameStorage _saver;
        private readonly ILogger _logger;

        public GamesRepository(IGameStorage saver, ILogger logger)
        {
            _saver = saver;
            _logger = logger;
        }

        private Dictionary<GameId, Game> _games = new Dictionary<GameId, Game>();

        public IReadOnlyDictionary<GameId, Game> Games => _games;

        public void Add(Game game)
        {
            if (_games.ContainsKey(game.Id))
            {
                throw new DuplicateKeyException("Game already added.");
            }

            _games.Add(game.Id, game);
            _saver.SaveGames(_games.Values.ToList());
        }

        public Game NewGame()
        {
            Game game = new Game();
            _games.Add(game.Id, game);
            _saver.SaveGames(_games.Values.ToList());
            return game;
        }

        public void Save()
        {
            _saver.SaveGames(_games.Values.ToList());
        }

        public void ClearAll()
        {
            _games = new Dictionary<GameId, Game>();
            Save();
        }

        public void Initialize(bool shouldUseTestData = true)
        {
            var savedGames = _saver.LoadGames();
            if (savedGames.Any())
            {
                if (savedGames.GroupBy(g => g.Id).Any(g => g.Count() > 1))
                {
                    _logger.Warning("Games read from storage contained duplicated keys that were ignored.");
                }
                savedGames.GroupBy(g => g.Id)
                    .Select(g => g.First())
                    .ToList()
                    .ForEach(g =>
                    {
                        _games.Add(g.Id, g);

                    });
            }
            else if (shouldUseTestData)
            {
                AddTestData();
            }
        }

        private void AddTestData()
        {
            var startDeals = new List<Deal>
            {
                new Deal
                {
                    Id = DealId.New(),
                    CroupierHand = new Hand(new List<CardType>{CardType.Queen}),
                    PlayerHand = new Hand(new List<CardType>{CardType.Five, CardType.Jack}),
                },
                new Deal
                {
                    Id = DealId.New(),
                    CroupierHand = new Hand(new List<CardType>{CardType.Ace}),
                    PlayerHand = new Hand(new List<CardType>{CardType.Two, CardType.Six}),
                }
            };
            var game = new Game
            {
                Id = GameId.New(),
                History = startDeals,
            };
            foreach (var deal in game.History)
            {
                game.EndDeal(deal.Id);
            }
            _games.Add(
                game.Id,
                game
            );
        }
    }
}