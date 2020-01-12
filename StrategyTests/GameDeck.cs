using Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrategyTests
{
    public class GameDeck
    {
        private static readonly Random Random = new Random(0); // constant seed for the ease of debugging
        private readonly int _numberOfDecks;
        private readonly double _penetrationRate;

        private Queue<CardType> _shuffledCards;
        private int _drawnCardsCounter = 0;

        public GameDeck(int numberOfDecks, double penetrationRate)
        {
            if (penetrationRate <= 0 || penetrationRate > 1)
            {
                throw new ArgumentException("Deck must have penetration rate between 0 (exclusive) and 1 (inclusive), got: " + penetrationRate);
            }
            _numberOfDecks = numberOfDecks;
            _penetrationRate = penetrationRate;
            Shuffle();
        }

        /// <summary>
        /// Draws another card from possibly multiple decks included. If penetration rate was reached, then first shuffles all cards.
        /// </summary>
        /// <returns></returns>
        public CardType DrawNext()
        {
            _drawnCardsCounter++;
            return _shuffledCards.Dequeue();
        }

        public bool CheckShuffle()
        {
            if (_drawnCardsCounter >= (int)(_numberOfDecks * 52 * _penetrationRate))
            {
                Shuffle();
                return true;
            }

            return false;
        }

        private void Shuffle()
        {
            List<CardType> cards = new List<CardType>();
            for (int i = 0; i < 4 * _numberOfDecks; i++)
            {
                cards.Add(CardType.Two);
                cards.Add(CardType.Three);
                cards.Add(CardType.Four);
                cards.Add(CardType.Four);
                cards.Add(CardType.Six);
                cards.Add(CardType.Seven);
                cards.Add(CardType.Eight);
                cards.Add(CardType.Nine);
                cards.Add(CardType.Ten);
                cards.Add(CardType.Jack);
                cards.Add(CardType.Queen);
                cards.Add(CardType.King);
                cards.Add(CardType.Ace);
            }
            _shuffledCards = new Queue<CardType>(cards.OrderBy(c => Random.Next()));
            _drawnCardsCounter = 0;
        }
    }
}
