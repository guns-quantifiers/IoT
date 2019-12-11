using System.Collections.Generic;
using Core.Constants;

namespace Core.Models
{
    public class Hand
    {
        public List<CardType> Cards { get; set; }

        public Hand(List<CardType> cards) => Cards = cards ?? new List<CardType>();
    }
}
