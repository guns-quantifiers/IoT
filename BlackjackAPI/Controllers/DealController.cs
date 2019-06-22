using BlackjackAPI.Exceptions;
using BlackjackAPI.Models;
using BlackjackAPI.Strategies;
using BlackjackAPI.Strategies.BetStrategy;
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
        public DealController(UstonSSGameContext gameContext, ILogger<DealController> logger, IStrategyProvider strategyProvider)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
            _strategyProvider = strategyProvider;
        }

        public UstonSSGameContext GameContext { get; }
        private readonly ILogger<DealController> _logger;
        private readonly IStrategyProvider _strategyProvider;
        private readonly BetMultiplierCalculator _betMultiplierCalculator = new BetMultiplierCalculator();

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

            var deal = game.History.Find(d => d.Id == dealId);
            return new OkObjectResult(new
            {
                strategy = _strategyProvider.Get(game, deal).ToString(),
                multiplier = _betMultiplierCalculator.Calculate(game, dealId)
            });
        }

        [HttpPost]
        [Route("end")]
        public IActionResult EndDeal([FromBody] EndDealModel model)
        {
            Guid dealId = new Guid(model.DealToken);
            var game = GameContext.Games.Values
                .ToList()
                .Find(g => g.History.Exists(d => d.Id == dealId));
            if (game == null)
            {
                throw new NotFoundException($"No game with deal with id {model?.DealToken}");
            }

            game.EndDeal(dealId);
            GameContext.Save();

            return new OkResult();
        }

        public class EndDealModel
        {
            public string DealToken { get; set; }
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

            if (deal.IsEnded)
            {
                throw new DealEndedException($"Cannot update already ended deal: {model?.DealToken}");
            }

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

        private bool ParseCardType(string s, out CardType card)
        {
            switch (s)
            {
                case "Two":
                    card = CardType.Two;
                    return true;
                case "Three":
                    card = CardType.Three;
                    return true;
                case "Four":
                    card = CardType.Four;
                    return true;
                case "Five":
                    card = CardType.Five;
                    return true;
                case "Six":
                    card = CardType.Six;
                    return true;
                case "Seven":
                    card = CardType.Seven;
                    return true;
                case "Eight":
                    card = CardType.Eight;
                    return true;
                case "Nine":
                    card = CardType.Nine;
                    return true;
                case "Ten":
                    card = CardType.Ten;
                    return true;
                case "Jack":
                    card = CardType.Jack;
                    return true;
                case "Queen":
                    card = CardType.Queen;
                    return true;
                case "King":
                    card = CardType.King;
                    return true;
                case "Ace":
                    card = CardType.Ace;
                    return true;
            }

            card = CardType.None;
            return false;
        }

        private DrawStrategy MockStrategy(Game game, Guid dealId)
        {
            return DrawStrategy.Hit;
        }
    }
}