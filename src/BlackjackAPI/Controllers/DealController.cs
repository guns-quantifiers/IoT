using Core.Components;
using Core.Constants;
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
    public class DealController
    {
        public DealController(IGameContext gameContext,
            ILogger<DealController> logger,
            IStrategyProvider strategyProvider,
            IBetMultiplierCalculator betMultiplierCalculator)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
            _strategyProvider = strategyProvider;
            _betMultiplierCalculator = betMultiplierCalculator;
        }

        public IGameContext GameContext { get; }
        private readonly ILogger<DealController> _logger;
        private readonly IStrategyProvider _strategyProvider;
        private readonly IBetMultiplierCalculator _betMultiplierCalculator;

        [HttpGet]
        [Route("strategy")]
        public IActionResult GetStrategy([FromQuery] string dealToken)
        {
            Guid dealId;
            try
            {
                dealId = new Guid(dealToken);
            }
            catch (Exception e)
            {
                throw new BlackjackBadRequestException($"Could not create deal id from: {dealToken}", e);
            }

            Game game;
            Deal deal;
            try
            {
                game = GameContext.Games.Values
                .ToList()
                .Find(g => g.History.Exists(d => d.Id == dealId));
                deal = game.History.Find(d => d.Id == dealId);
            }
            catch (Exception e)
            {
                throw new BlackjackBadRequestException($"Could not find deal for {dealToken}. ", e);
            }

            DrawStrategy strategy = DrawStrategy.None;
            BetMultiplier multiplier;
            try
            {
                strategy = _strategyProvider.Get(game, deal);
                multiplier = _betMultiplierCalculator.Calculate(game, dealId);
            }
            catch (Exception e)
            {
                if (strategy != DrawStrategy.None)
                {
                    throw new StrategyException($"Could not get strategy multiplier for {dealToken}, although got strategy: {strategy}. Check logs for more information.", e);
                }
                throw new StrategyException($"Could not get strategy for {dealToken}. Check logs for more information.", e);
            }
            return new OkObjectResult(new
            {
                Strategy = strategy.ToString(),
                Multiplier = multiplier?.Value.ToString("F2")
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

            return new OkObjectResult(new
            {
                Message = "Ok"
            });
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

            deal.PlayerHand.Cards = new List<CardType>(
                model.PlayerHand.Select(s =>
                {
                    if (ParseCardType(s, out CardType card))
                    {
                        return card;
                    }

                    throw new ApplicationException($"Invalid card type: {s}");
                }));
            deal.CroupierHand.Cards = new List<CardType>(
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
    }
}