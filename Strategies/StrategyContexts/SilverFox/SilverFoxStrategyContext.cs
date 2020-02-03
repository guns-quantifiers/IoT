using Core.Components;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.SilverFox
{
    public class SilverFoxStrategyContext : BaseStrategyContext, IStrategyContext
    {
        public SilverFoxStrategyContext(int deckAmount, bool useTrueCounter) : base(deckAmount, new SilverFoxCardCounter(), useTrueCounter)
        {
        }


        protected override double GetRunningCounter(Game game) => game.History
            .Where(d => d.IsEnded)
            .Aggregate(0d, (sum, nextDeal) => sum + CardCounter.Count(nextDeal));
    }
}