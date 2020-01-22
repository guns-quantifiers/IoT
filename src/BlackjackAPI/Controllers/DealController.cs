using BlackjackAPI.Services;
using Core.Components;
using Core.Constants;
using Core.Exceptions;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Strategies;
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
            StrategiesResolver strategiesResolver)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _logger = logger;
            _strategyProvider = strategyProvider;
            _strategiesResolver = strategiesResolver;
        }

        public IGamesRepository GameContext { get; }
        private readonly ILogger<DealController> _logger;
        private readonly IStrategyProvider _strategyProvider;
        private readonly StrategiesResolver _strategiesResolver;

        [HttpGet]
        [Route("strategy")]
        public IActionResult GetStrategy([FromQuery] string dealToken)
        {
            DealId dealId = dealToken.ToDealId();
            Deal deal;
            Game game;
            try
            {
                GameContext.TryFindGameForDeal(dealId, out game);
                deal = game.History.Find(d => d.Id == dealId);
            }
            catch (Exception e)
            {
                throw new BlackjackBadRequestException($"Could not find deal for {dealToken}. ", e);
            }

            DrawStrategy strategy = DrawStrategy.None;
            try
            {
                strategy = _strategyProvider.Get(game, deal);
                StrategyInfo strategyInfo = _strategiesResolver.GetStrategy(game, deal);
                return new OkObjectResult(new
                {
                    Strategy = strategy.ToString(),
                    Counter = strategyInfo.Counter,
                    Multiplier = strategyInfo.BetMultiplier?.Value.ToString("F2")
                });
            }
            catch (Exception e)
            {
                if (strategy != DrawStrategy.None)
                {
                    throw new StrategyException($"Could not get strategy multiplier for {dealToken}, although got strategy: {strategy}. Check logs for more information.", e);
                }
                throw new StrategyException($"Could not get strategy for {dealToken}. Check logs for more information.", e);
            }
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
            GameContext.TryFindGameForDeal(dealId, out Game game);
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
            GameContext.TryFindGameForDeal(dealId, out Game game);
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