using BlackjackAPI.Models;
using Core.Components;
using Core.Constants;
using Core.Exceptions;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies.GameContexts
{
    public class UstonSSGameContext : IGameContext
    {
        private readonly IGameSaver _saver;
        private readonly ILogger _logger;
        private readonly int _deckAmount;
        private readonly IDealCardCounter _cardCounter = new UstonSSCardCounter();

        public UstonSSGameContext(IGameSaver saver,
            ILogger logger,
            int deckAmount = 2)
        {
            _saver = saver;
            _logger = logger;
            _deckAmount = deckAmount;
        }

        private Dictionary<Guid, Game> _games = new Dictionary<Guid, Game>();

        public IReadOnlyDictionary<Guid, Game> Games => _games;

        public void Add(Game game)
        {
            if (_games.ContainsKey(game.Id))
            {
                throw new DuplicateKeyException("Game already added.");
            }

            game.CardCounter = -2 * _deckAmount;
            _games.Add(game.Id, game);
            _saver.SaveGames(_games.Values.ToList());
        }

        public Game NewGame()
        {
            Game game = new Game
            {
                CardCounter = -2 * _deckAmount,
                DealCardCounter = _cardCounter
            };
            _games.Add(game.Id, game);
            _saver.SaveGames(_games.Values.ToList());
            return game;
        }

        public void Add(Guid gameId, Deal deal)
        {
            if (_games.TryGetValue(gameId, out Game currentGame))
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

        public void ClearAll()
        {
            _games = new Dictionary<Guid, Game>();
            Save();
        }

        public void Initialize(bool shouldUseTestData = true)
        {
            var savedGames = _saver.LoadGames();
            if (savedGames.Any())
            {
                if (savedGames.GroupBy(g => g.Id).Any(g => g.Count() > 1))
                {
                    _logger.Warning("Games read from file contained duplicated keys that were ignored.");
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
                new Deal()
                {
                    Id = Guid.NewGuid(),
                    CroupierHand = new List<CardType>{CardType.Queen},
                    PlayerHand = new List<CardType>{CardType.Five, CardType.Jack},
                },
                new Deal()
                {
                    Id = Guid.NewGuid(),
                    CroupierHand = new List<CardType>{CardType.Ace},
                    PlayerHand = new List<CardType>{CardType.Two, CardType.Six},
                }
            };
            var game = new Game()
            {
                Id = Guid.NewGuid(),
                History = startDeals,
                CardCounter = -2 * _deckAmount,
                DealCardCounter = _cardCounter
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