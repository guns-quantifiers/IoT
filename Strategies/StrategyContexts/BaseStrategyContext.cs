using Core.Components;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts
{
    public abstract class BaseStrategyContext
    {
        protected readonly IDealCardCounter CardCounter;
        protected readonly int DeckAmount;
        private readonly bool _useTrueCounter;

        protected BaseStrategyContext(int deckAmount, IDealCardCounter cardCounter, bool useTrueCounter)
        {
            DeckAmount = deckAmount;
            CardCounter = cardCounter;
            _useTrueCounter = useTrueCounter;
        }
        public double GetCounter(Game game, Deal deal)
            => GetTrueOrRunningCounter(game, deal);

        protected abstract double GetRunningCounter(Game game);
        private double GetTrueOrRunningCounter(Game game, Deal deal)
        {
            var runningCounter = GetRunningCounter(game) + CardCounter.Count(deal);
            var trueCounterDivisor = _useTrueCounter ?
                DeckAmount - (game.History.Where(d => d.IsEnded).Aggregate(
                    deal.CroupierHand.Cards.Count + deal.PlayerHand.Cards.Count,
                    (sum, d) => sum + d.CroupierHand.Cards.Count + d.PlayerHand.Cards.Count) / 52)
                : 1;
            return runningCounter / trueCounterDivisor;
        }
    }
}
