using Core.Components;
using Core.Constants;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.Knockout
{
    public class KnockoutCardCounter : IDealCardCounter
    {
        public double Count(Deal deal)
        {
            return deal.PlayerHand.Cards.Sum(c => c.KnockoutCounter())
                   + deal.CroupierHand.Cards.Sum(c => c.KnockoutCounter());
        }
    }

    internal static class KnockoutCardExtensions
    {
        public static int KnockoutCounter(this CardType card)
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