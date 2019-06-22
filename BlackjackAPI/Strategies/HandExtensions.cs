using BlackjackAPI.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BlackjackAPI.Strategies
{
    public static class HandExtensions
    {
        public static int Sum(this IReadOnlyList<CardType> hand)
        {
            return hand.Select(Value).Sum();
        }

        public static int SafeSum(this IReadOnlyList<CardType> hand)
        {
            var sumWithAcesAs11 = hand.Select(Value).Sum();
            if (sumWithAcesAs11 <= 21)
            {
                return sumWithAcesAs11;
            }
            else
            {
                var acesAmount = hand.Count(c => c == CardType.Ace);
                int currentSum = sumWithAcesAs11;
                while (acesAmount > 0 && currentSum > 21)
                {
                    currentSum -= 10;
                    acesAmount--;
                }

                return currentSum;
            }
        }

        public static int SumWithOneAceAs1(this IReadOnlyList<CardType> hand)
        {

            return hand.Select(Value).Sum()
                - (hand.Contains(CardType.Ace) ? 10 : 0);
        }

        public static int Value(this CardType card)
        {
            switch (card)
            {
                case CardType.Two:
                    return 2;
                case CardType.Three:
                    return 3;
                case CardType.Four:
                    return 4;
                case CardType.Five:
                    return 5;
                case CardType.Six:
                    return 6;
                case CardType.Seven:
                    return 7;
                case CardType.Eight:
                    return 8;
                case CardType.Nine:
                    return 9;
                case CardType.Ten:
                    return 10;
                case CardType.Jack:
                    return 10;
                case CardType.Queen:
                    return 10;
                case CardType.King:
                    return 10;
                case CardType.Ace:
                    return 11;
            }

            return 0;
        }

        public static DrawStrategy ToStrategy(this int strategyCode)
        {
            switch (strategyCode)
            {
                case 1:
                    return DrawStrategy.Hit;
                case 2:
                    return DrawStrategy.Stand;
                case 3: // TODO: Split
                    Debug.WriteLine("Got strategy code 3, so translated to stand.");
                    return DrawStrategy.Hit;
                case 4:
                    return DrawStrategy.DoubleDownOrHit;
                case 5:
                    return DrawStrategy.DoubleDownOrStand;
                default:
                    return DrawStrategy.Stand;
            }
        }
    }
}
