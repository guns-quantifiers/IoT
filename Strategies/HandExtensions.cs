using Core.Constants;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
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
            return card switch
            {
                CardType.Two => 2,
                CardType.Three => 3,
                CardType.Four => 4,
                CardType.Five => 5,
                CardType.Six => 6,
                CardType.Seven => 7,
                CardType.Eight => 8,
                CardType.Nine => 9,
                CardType.Ten => 10,
                CardType.Jack => 10,
                CardType.Queen => 10,
                CardType.King => 10,
                CardType.Ace => 11,

                _ => 0,
            };
        }

        public static DrawStrategy ToStrategy(this int strategyCode)
        {
            return strategyCode switch
            {
                1 => DrawStrategy.Hit,
                2 => DrawStrategy.Stand,
                3 => DrawStrategy.Hit,
                4 => DrawStrategy.DoubleDownOrHit,
                5 => DrawStrategy.DoubleDownOrStand,
                _ => DrawStrategy.Stand,
            };
        }
    }
}
