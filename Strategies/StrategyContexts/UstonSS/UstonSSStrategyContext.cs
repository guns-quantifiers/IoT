using Core.Components;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.UstonSS
{
    public class UstonSSStrategyContext : BaseStrategyContext, IStrategyContext
    {
        public UstonSSStrategyContext(int deckAmount, bool useTrueCounter) : base(deckAmount, new UstonSSCardCounter(), useTrueCounter)
        {
        }

        protected override double GetRunningCounter(Game game) => game.History
            .Where(d => d.IsEnded)
            .Aggregate(-4d * DeckAmount, (sum, nextDeal) => sum + CardCounter.Count(nextDeal));
    }
}