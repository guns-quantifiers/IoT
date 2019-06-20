using System.Collections;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class Hand : IEnumerable
    {
        public List<CardType> Cards { get; set; } = new List<CardType>();

        public IEnumerator GetEnumerator()
            => Cards.GetEnumerator();

        public void Add(CardType card)
        {
            Cards.Add(card);
        }
    }
}
