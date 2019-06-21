using BlackjackAPI.Exceptions;
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
    public class DealController
    {
        public DealController(GameContext gameContext, ILogger<DealController> logger)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
        }

        public GameContext GameContext { get; }
        private readonly ILogger<DealController> _logger;
        
        [HttpGet]
        [Route("strategy")]
        public IActionResult GetStrategy([FromQuery] string dealToken)
        {
            Guid dealId = new Guid(dealToken);
            var game = GameContext.Games.Values
                .ToList()
                .Find(g => g.History.Exists(d => d.Id == dealId));
            if (game == null)
            {
                throw new NotFoundException("Game for given token not found");
            }

            return new OkObjectResult(new
            {
                strategy = MockStrategy(game, dealId).ToString()
            });
        }

        private Strategy MockStrategy(Game game, Guid dealId)
        {
            return Strategy.Draw;
        }


        [HttpPost]
        [Route("update")]
        public IActionResult UpdateDeal([FromBody] UpdateDealModel model)
        {
            Guid dealId = new Guid(model.DealToken);
            var game = GameContext.Games.Values
                .ToList()
                .Find(g => g.History.Exists(d => d.Id == dealId));
            if (game == null)
            {
                throw new NotFoundException($"No game with deal with {model?.DealToken}");
            }
            var deal = game.History.First(d => d.Id == dealId);

            deal.PlayerHand = new List<CardType>(
                model.PlayerHand.Select(s =>
                {
                    if (ParseCardType(s, out CardType card))
                    {
                        return card;
                    }

                    throw new ApplicationException($"Invalid card type: {s}");
                }));
            deal.CroupierHand = new List<CardType>(
                model.CroupierHand.Select(s =>
                {
                    if (ParseCardType(s, out CardType card))
                    {
                        return card;
                    }

                    throw new ApplicationException($"Invalid card type: {s}");
                }));

            GameContext.Save();

            return new OkResult();
        }

        public class UpdateDealModel
        {
            public string DealToken { get; set; }
            public List<string> PlayerHand { get; set; }
            public List<string> CroupierHand { get; set; }
        }

        public bool ParseCardType(string s, out CardType card)
        {
            switch (s)
            {
                case "2":
                    card = CardType.Two;
                    return true;
                case "3":
                    card = CardType.Two;
                    return true;
                case "4":
                    card = CardType.Two;
                    return true;
                case "5":
                    card = CardType.Two;
                    return true;
                case "6":
                    card = CardType.Two;
                    return true;
                case "7":
                    card = CardType.Two;
                    return true;
                case "8":
                    card = CardType.Two;
                    return true;
                case "9":
                    card = CardType.Two;
                    return true;
                case "10":
                    card = CardType.Two;
                    return true;
                case "J":
                    card = CardType.Two;
                    return true;
                case "Q":
                    card = CardType.Two;
                    return true;
                case "K":
                    card = CardType.Two;
                    return true;
                case "A":
                    card = CardType.Two;
                    return true;
            }

            card = CardType.None;
            return false;
        }
    }
}