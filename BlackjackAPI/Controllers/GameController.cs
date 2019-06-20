using BlackjackAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Optional;
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
        public Option<GameContext.Success, Error> Post()
        {
            _logger.LogInformation("New game creation POST accepted.");
            var newGame = new Game();
            string r
            GameContext.Add(newGame)
                .Map(
                    some: s =>  new
                    {
                        s.Message
                    },
                    e => return new
                    {
                        e.Message
                    });
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