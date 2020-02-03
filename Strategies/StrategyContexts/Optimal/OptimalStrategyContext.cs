using Core.Components;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.Optimal
{
    public class OptimalStrategyContext : BaseStrategyContext, IStrategyContext
    {
        public OptimalStrategyContext(int deckAmount, bool useTrueCounter) : base(deckAmount, new OptimalCardCounter(), useTrueCounter)
        {
        }

        protected override double GetRunningCounter(Game game) => game.History
            .Where(d => d.IsEnded)
            .Aggregate(0d, (sum, nextDeal) => sum + CardCounter.Count(nextDeal));
    }
}