using Core.Components;
using Core.Constants;
using Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class ChartedBasicStrategy : IStrategyProvider
    {
        private readonly int[,] HardHandsChart = new[,] //22,10
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //0
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //1
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //2
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //3
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //4
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //5
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //6
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //7
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //8

            {1, 1, 4, 4, 4, 4, 1, 1, 1, 1}, //9
            {1, 4, 4, 4, 4, 4, 4, 4, 4, 1}, //10
            {1, 4, 4, 4, 4, 4, 4, 4, 4, 4}, //11
            {1, 1, 1, 2, 2, 2, 1, 1, 1, 1}, //12
            {1, 2, 2, 2, 2, 2, 1, 1, 1, 1}, //13
            {1, 2, 2, 2, 2, 2, 1, 1, 1, 1}, //14
            {1, 2, 2, 2, 2, 2, 1, 1, 1, 1}, //15

            {1, 2, 2, 2, 2, 2, 1, 1, 1, 1}, //16
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2}, //17
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2}, //18
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2}, //19
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2}, //20
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2}, //21
        };

        private readonly int[,] SoftHandsChart = new[,] //22,10
        {
            {1,1,1,1,4,4,1,1,1,1}, //A-0
            {1,1,1,1,4,4,1,1,1,1}, //A-1
            {1,1,1,1,4,4,1,1,1,1}, //A-2
            {1,1,1,1,4,4,1,1,1,1}, //A-3
            {1,1,1,4,4,4,1,1,1,1}, //A-4
            {1,1,1,4,4,4,1,1,1,1}, //A-5
            {1,1,4,4,4,4,1,1,1,1}, //A-6
            {1,2,5,5,5,5,2,2,1,1}, //A-7
            {2,2,2,2,2,2,2,2,2,2}, //A-8
            {2,2,2,2,2,2,2,2,2,2}, //A-9
            {2,2,2,2,2,2,2,2,2,2}, //A-10
        };

        public DrawStrategy Get(Game game, Deal deal)
        {
            var (playerHand, croupierHand) = deal;
            if (playerHand.Contains(CardType.Ace))
            {
                return HandleSoftHand(playerHand, croupierHand);
            }
            return HandleHardHand(playerHand, croupierHand);
        }

        private DrawStrategy HandleHardHand(IReadOnlyList<CardType> playerHand, IReadOnlyList<CardType> croupierHand)
        {
            return playerHand.SafeSum() >= 21
                ? DrawStrategy.Stand
                : HardHandsChart[playerHand.SafeSum(), croupierHand.SumWithOneAceAs1() - 1].ToStrategy();
        }

        private DrawStrategy HandleSoftHand(IReadOnlyList<CardType> playerHand, IReadOnlyList<CardType> croupierHand)
        {
            if (playerHand.SafeSum() >= 21)
                return DrawStrategy.Stand;

            if (playerHand.SumWithOneAceAs1() > 10)
            {
                // we cannot currently do anything more thoughtful so
                return HandleHardHand(playerHand, croupierHand);
            }

            return SoftHandsChart[playerHand.SumWithOneAceAs1(), croupierHand.Sum() - 2].ToStrategy();
        }
    }
}
