using Core.Components;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.Knockout
{
    public class KnockoutStrategyContext : IStrategyContext
    {
        private readonly IDealCardCounter _cardCounter = new KnockoutCardCounter();

        public KnockoutStrategyContext()
        {
        }

        public int GetCounter(Game game, Deal deal)
            => GetCounter(game) + _cardCounter.Count(deal);

        public int GetCounter(Game game) => game.History
            .Where(d => d.IsEnded)
            .Aggregate(0, (sum, nextDeal) => sum + _cardCounter.Count(nextDeal));
    }
}