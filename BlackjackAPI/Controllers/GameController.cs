using BlackjackAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
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

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}