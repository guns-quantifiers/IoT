using BlackjackAPI.Services;
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
        public DealController(IGamesRepository gameContext,
            ILogger<DealController> logger,
            IStrategyProvider strategyProvider,
            IBetMultiplierCalculator betMultiplierCalculator, IStrategyContext strategyContext)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
            _strategyProvider = strategyProvider;
            _betMultiplierCalculator = betMultiplierCalculator;
            _strategyContext = strategyContext;
        }

        public IGamesRepository GameContext { get; }
        private readonly ILogger<DealController> _logger;
        private readonly IStrategyProvider _strategyProvider;
        private readonly IStrategyContext _strategyContext;
        private readonly IBetMultiplierCalculator _betMultiplierCalculator;

        [HttpGet]
        [Route("strategy")]
        public IActionResult GetStrategy([FromQuery] string dealToken)
        {
            DealId dealId = dealToken.ToDealId();
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
                multiplier = _betMultiplierCalculator.Calculate(_strategyContext.GetCounter(game, deal));
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
            if (model == null)
            {
                throw new BlackjackBadRequestException($"Could not parse request model on {nameof(EndDeal)} endpoint.");
            }

            DealId dealId = model.DealToken.ToDealId();
            var game = GameContext.Games.Values
                .ToList()
                .Find(g => g.History.Exists(d => d.Id == dealId));
            if (game == null)
            {
                throw new NotFoundException($"No game with deal with id {model?.DealToken}");
            }

            game.EndDeal(dealId);
            GameContext.Update(game);

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
            if (model == null)
            {
                throw new BlackjackBadRequestException($"Could not parse request model on {nameof(UpdateDeal)} endpoint.");
            }

            DealId dealId = model.DealToken.ToDealId();
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

            deal.PlayerHand.Cards = new List<CardType>(model.PlayerHand.Select(ParseCardType));
            deal.CroupierHand.Cards = new List<CardType>(model.CroupierHand.Select(ParseCardType));

            GameContext.Update(game);
            return new OkResult();
        }

        public class UpdateDealModel
        {
            public string DealToken { get; set; }
            public List<string> PlayerHand { get; set; }
            public List<string> CroupierHand { get; set; }
        }

        private CardType ParseCardType(string cardString)
        {
            if (Enum.TryParse(cardString, out CardType parsedCard))
            {
                return parsedCard;
            }
            throw new ApplicationException($"Unknown card type: {cardString}");
        }
    }
}