using BlackjackAPI.Controllers.Models;
using BlackjackAPI.Services;
using Core.Components;
using Core.Constants;
using Core.Exceptions;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Strategies;
using StrategyTests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameController
    {
        public GameController(IGamesRepository gameContext, ILogger<GameController> logger)
        {
            _gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
        }

        private readonly IGamesRepository _gameContext;
        private readonly ILogger<GameController> _logger;

        [HttpGet]
        [Route("")]
        public ActionResult<List<Game>> Get()
        {
            return _gameContext.GetAll().ToList();
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateGame()
        {
            _logger.LogInformation("New game creation POST accepted.");
            try
            {
                var newGame = _gameContext.NewGame();
                return new OkObjectResult(new
                {
                    gameToken = newGame.Id.ToString()
                });
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Cannot create game: {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("addDeal")]
        public IActionResult AddDeal([FromBody] AddDealModel model)
        {
            if (model == null)
            {
                throw new BlackjackBadRequestException($"Could not parse request model on {nameof(AddDeal)} endpoint.");
            }

            GameId gameId = model.GameToken.ToGameId();
            if (_gameContext.TryFindOne(gameId, out Game game))
            {
                _logger.LogInformation($"New add deal POST accepted for game {model.GameToken}");
                var deal = game.NewDeal();
                _gameContext.Update(game);
                return new OkObjectResult(new
                {
                    dealToken = deal.Id.ToString()
                });
            }

            return new NotFoundObjectResult(new
            {
                Message = $"Game with id {model.GameToken} not found."
            });
        }

        public class AddDealModel
        {
            public string GameToken { get; set; }
        }

        [HttpDelete]
        [Route("clearAll")]
        public IActionResult ClearAll()
        {
            _gameContext.ClearAll();
            return new OkResult();
        }


        [HttpPost]
        [Route("generate")]
        public List<GamesGenerationSingleResult> Generate(GamesGenerateParameters parameters)
        {
            var parametersProperties = typeof(GamesGenerateParameters).GetProperties().Select(p => p.GetValue(parameters));
            if (parametersProperties.Any(p => p == null))
            {
                throw new ArgumentException("You need to specify all parameters for games auto generation.");
            }

            List<PlayerDecision> generationtResults = new TestCaseGenerator().Generate(new TestCaseSettings(
                parameters.NumberOfDecks,
                parameters.CountingStrategy,
                5,
                parameters.DeckPenetration,
                parameters.GamesToGenerate,
                parameters.BetStrategy.TryBindCalculatorConfiguration(),
                parameters.Seed));

            return generationtResults.Select(r =>
            {
                var currentDeal = r.GameSnapshot.History.LastOrDefault();
                return new GamesGenerationSingleResult()
                {
                    GameID = r.GameSnapshot.Id.ToString(),
                    BetMultiplier = r.BetMultiplier,
                    Counter = r.Counter,
                    Decision = r.Type,
                    Deal = new DealSnapshotModel
                    {
                        ID = currentDeal?.Id.ToString(),
                        Croupier = currentDeal?.CroupierHand.Cards,
                        Player = currentDeal?.PlayerHand.Cards
                    }
                };
            }).ToList();
        }

        public class GamesGenerateParameters
        {
            public SetCountingStrategyModel CountingStrategy { get; set; }
            public SetBetStrategyModel BetStrategy { get; set; }
            public int NumberOfDecks { get; set; }
            public double DeckPenetration { get; set; }
            public int GamesToGenerate { get; set; }
            public int Seed { get; set; } = new Random().Next();
        }

        public class GamesGenerationSingleResult
        {
            public string GameID { get; set; }
            public DealSnapshotModel Deal { get; set; }
            public int Counter { get; set; }
            public double BetMultiplier { get; set; }
            public DrawStrategy Decision { get; set; }
        }

        public class DealSnapshotModel
        {
            public string ID { get; set; }
            public List<CardType> Croupier { get; set; }
            public List<CardType> Player { get; set; }
        }
    }
}