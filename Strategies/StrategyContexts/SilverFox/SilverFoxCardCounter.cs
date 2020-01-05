using Core.Components;
using Core.Constants;
using Core.Models;
using System.Linq;

namespace Strategies.StrategyContexts.SilverFox
{
    public class SilverFoxCardCounter : IDealCardCounter
    {
        public int Count(Deal deal)
        {
            return deal.PlayerHand.Cards.Sum(c => c.SilverFoxCounter())
                   + deal.CroupierHand.Cards.Sum(c => c.SilverFoxCounter());
        }
    }

    internal static class SilverFoxCardExtensions
    {
        public static int SilverFoxCounter(this CardType card)
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
                    return 0;
                case CardType.Nine:
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