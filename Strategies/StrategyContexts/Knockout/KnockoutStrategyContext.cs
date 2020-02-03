﻿using Core.Components;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.Knockout
{
    public class KnockoutStrategyContext : BaseStrategyContext, IStrategyContext
    {
        public KnockoutStrategyContext(int deckAmount, bool useTrueCounter) : base(deckAmount, new KnockoutCardCounter(), useTrueCounter)
        {
        }

        protected override double GetRunningCounter(Game game) => game.History
            .Where(d => d.IsEnded)
            .Aggregate(0d, (sum, nextDeal) => sum + CardCounter.Count(nextDeal));
    }
}