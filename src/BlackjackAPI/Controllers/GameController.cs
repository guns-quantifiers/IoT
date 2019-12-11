using Core.Components;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackAPI.Services;
using Core.Exceptions;

namespace BlackjackAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameController
    {
        public GameController(IGamesRepository gameContext, ILogger<GameController> logger)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
        }

        public IGamesRepository GameContext { get; }
        private readonly ILogger<GameController> _logger;

        [HttpGet]
        [Route("")]
        public ActionResult<List<Game>> Get()
        {
            return GameContext.Games.Values.ToList();
        }
        
        [HttpPost]
        [Route("create")]
        public IActionResult CreateGame()
        {
          _logger.LogInformation("New game creation POST accepted.");
            try
            {
                var newGame = GameContext.NewGame();
                return new OkObjectResult(new
                {
                    gameToken = newGame.Id
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

            var gameId = model.GameToken.ToGameId();
            if (GameContext.Games.TryGetValue(gameId, out Game game))
            {
                _logger.LogInformation($"New add deal POST accepted for game {model.GameToken}");
                var deal = game.NewDeal();
                return new OkObjectResult(new
                {
                    dealToken = deal.Id
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
            GameContext.ClearAll();
            return new OkResult();
        }
    }
}