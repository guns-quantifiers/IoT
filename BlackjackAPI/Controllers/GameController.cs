using BlackjackAPI.Models;
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
        public GameController(GameContext gameContext, ILogger<GameController> logger)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
        }

        public GameContext GameContext { get; }
        private readonly ILogger<GameController> _logger;

        [HttpGet]
        public ActionResult<List<Game>> Get()
        {
            return GameContext.Games.Values.ToList();
        }
        
        [HttpPost]
        [Route("create")]
        public IActionResult CreateGame()
        {
          _logger.LogInformation("New game creation POST accepted.");
            var newGame = new Game();
            try
            {
                GameContext.Add(newGame);
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
            var gameId = new Guid(model.GameToken);
            if(GameContext.Games.TryGetValue(gameId, out Game game))
            {
                _logger.LogInformation($"New add deal POST accepted for game {model.GameToken}");
                var deal = new Deal();
                game.History.Add(deal);
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