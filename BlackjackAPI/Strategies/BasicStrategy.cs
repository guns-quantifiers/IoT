using BlackjackAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackAPI.Strategies
{
    public class BasicStrategy
    {
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
            if (playerHand.Sum() <= 8)
            {
                return DrawStrategy.Hit;
            }
            
            switch (playerHand.Sum())
            {
                case 9:
                    if (croupierHand.Sum() > 2 && croupierHand.Sum() <= 5)
                    {
                        return DrawStrategy.DoubleDown;
                    }

                    return DrawStrategy.Hit;
                case 10:
                    if (croupierHand.Sum() >= 2 && croupierHand.Sum() <= 9)
                    {
                        return DrawStrategy.DoubleDown;
                    }

                    return DrawStrategy.Hit;
                case 11:
                    if (croupierHand.Sum() >= 2 && croupierHand.Sum() <= 10)
                    {
                        return DrawStrategy.DoubleDown;
                    }
                    //then dealer has ace
                    return DrawStrategy.Hit;
                case 12:
                    if (croupierHand.Sum() >= 4 && croupierHand.Sum() <= 6)
                    {
                        return DrawStrategy.Stand;
                    }
                    return DrawStrategy.Hit;
                case 13:
                case 14:
                case 15:
                case 16:
                    if (croupierHand.Sum() >= 2 && croupierHand.Sum() <= 6)
                    {
                        return DrawStrategy.Stand;
                    }
                    return DrawStrategy.Hit;
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                    return DrawStrategy.Stand;
                default:
                    return DrawStrategy.Stand;
            }
        }

        private DrawStrategy HandleSoftHand(IReadOnlyList<CardType> playerHand, IReadOnlyList<CardType> croupierHand)
        {
            if (playerHand.Sum() <= 12)
            {
                return DrawStrategy.Hit;
            }

            switch (playerHand.Sum())
            {
                case 13:
                case 14:
                    if (croupierHand.Sum() == 5 || croupierHand.Sum() == 6)
                    {
                        return DrawStrategy.DoubleDown;
                    }

                    return DrawStrategy.Hit;
                case 15:
                case 16:
                    if (croupierHand.Sum() >= 4 && croupierHand.Sum() <= 6)
                    {
                        return DrawStrategy.DoubleDown;
                    }

                    return DrawStrategy.Hit;
                case 17:
                    if (croupierHand.Sum() >= 3 && croupierHand.Sum() <= 6)
                    {
                        return DrawStrategy.DoubleDown;
                    }
                    return DrawStrategy.Hit;
                case 18:
                    switch (croupierHand.Sum())
                    {
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            return DrawStrategy.DoubleDown;
                        case 9:
                        case 10:
                        case 11:
                            return DrawStrategy.DoubleDown;
                        case 2:
                        case 7:
                        case 8:
                        default:
                            return DrawStrategy.Stand;
                    }
                case 19:
                case 20:
                case 21:
                    return DrawStrategy.Stand;
                default:
                    return DrawStrategy.Stand;
            }
        }
    }
}
