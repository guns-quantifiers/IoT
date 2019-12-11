using System.Linq;
using Core.Components;
using Core.Models;

namespace Strategies.StrategyContexts
{
    public class UstonSSStrategyContext : IStrategyContext
    {
        private readonly int _deckAmount;
        private readonly IDealCardCounter _cardCounter = new UstonSSCardCounter();

        public UstonSSStrategyContext(int deckAmount = 2)
        {
            _deckAmount = deckAmount;
        }

        public int GetCounter(Game game, Deal deal)
            => GetCounter(game) + _cardCounter.Count(deal);

        public int GetCounter(Game game) => game.History
            .Where(d => d.IsEnded)
            .Aggregate(-4 * _deckAmount, (sum, nextDeal) => sum + _cardCounter.Count(nextDeal));
    }
}