using Core.Components;
using Core.Constants;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.UstonSS
{
    public class UstonSSCardCounter : IDealCardCounter
    {
        public int Count(Deal deal)
        {
            return deal.PlayerHand.Cards.Sum(c => c.UstonCounter())
                   + deal.CroupierHand.Cards.Sum(c => c.UstonCounter());
        }
    }

    internal static class UstonSSCardExtensions
    {
        public static int UstonCounter(this CardType card)
        {
            switch (card)
            {
                case CardType.Two:
                case CardType.Four:
                case CardType.Three:
                case CardType.Six:
                    return 2;
                case CardType.Five:
                    return 3;
                case CardType.Seven:
                    return 1;
                case CardType.Eight:
                    return 0;
                case CardType.Nine:
                    return -1;
                case CardType.Ten:
                case CardType.Jack:
                case CardType.Queen:
                case CardType.King:
                case CardType.Ace:
                    return -2;
                default:
                    return 0;
            }
        }
    }
}