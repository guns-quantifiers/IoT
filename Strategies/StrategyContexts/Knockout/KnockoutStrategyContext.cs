using Core.Components;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.Knockout
{
    public class KnockoutStrategyContext : BaseStrategyContext, IStrategyContext
    {
        public KnockoutStrategyContext(int deckAmount, bool useTrueCounter) : base(deckAmount, new KnockoutCardCounter(), useTrueCounter)
        {
        }

        protected override int GetRunningCounter(Game game) => game.History
            .Where(d => d.IsEnded)
            .Aggregate(0, (sum, nextDeal) => sum + CardCounter.Count(nextDeal));
    }
}