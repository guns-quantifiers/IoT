using Core.Components;
using Core.Constants;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.Optimal
{
    public class OptimalCardCounter : IDealCardCounter
    {
        public double Count(Deal deal)
        {
            return deal.PlayerHand.Cards.Sum(c => c.OptimalCounter())
                   + deal.CroupierHand.Cards.Sum(c => c.OptimalCounter());
        }
    }

    internal static class OptimalCardExtensions
    {
        public static double OptimalCounter(this CardType card)
        {
            switch (card)
            {
                case CardType.Two:
                    return 0.82;
                case CardType.Four:
                    return 0.94;
                case CardType.Three:
                    return 1.21;
                case CardType.Six:
                    return 1.52;
                case CardType.Five:
                    return 0.98;
                case CardType.Seven:
                    return 0.57;
                case CardType.Eight:
                    return -0.06;
                case CardType.Nine:
                    return -0.42;
                case CardType.Ten:
                case CardType.Jack:
                case CardType.Queen:
                case CardType.King:
                    return -1.07;
                case CardType.Ace:
                    return -1.28;
                default:
                    return 0;
            }
        }
    }
}