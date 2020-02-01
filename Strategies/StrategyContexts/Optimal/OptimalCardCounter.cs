using Core.Components;
using Core.Constants;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.Optimal
{
    public class OptimalCardCounter : IDealCardCounter
    {
        public int Count(Deal deal)
        {
            return deal.PlayerHand.Cards.Sum(c => c.OptimalCounter())
                   + deal.CroupierHand.Cards.Sum(c => c.OptimalCounter());
        }
    }

    internal static class OptimalCardExtensions
    {
        public static int OptimalCounter(this CardType card)
        {
            switch (card)
            {
                case CardType.Two:
                case CardType.Four:
                case CardType.Three:
                case CardType.Six:
                case CardType.Five:
                case CardType.Seven:
                    return 1;
                case CardType.Eight:
                case CardType.Nine:
                    return 0;
                case CardType.Ten:
                case CardType.Jack:
                case CardType.Queen:
                case CardType.King:
                case CardType.Ace:
                    return -1;
                default:
                    return 0;
            }
        }
    }
}