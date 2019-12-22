using BlackjackAPI.Services;
using Core.Components;
using Core.Exceptions;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            return _gameContext.Games.Values.ToList();
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
            if (_gameContext.Games.TryGetValue(gameId, out Game game))
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
    }
}