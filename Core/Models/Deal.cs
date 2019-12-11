using Core.Constants;
using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Deal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Hand PlayerHand { get; set; } = new Hand(new List<CardType>());
        public Hand CroupierHand { get; set; } = new Hand(new List<CardType>());
        public bool IsEnded { get; set; } = false;

        public void Deconstruct(out IReadOnlyList<CardType> playerHand, out IReadOnlyList<CardType> croupierHand)
        {
            playerHand = PlayerHand.Cards;
            croupierHand = CroupierHand.Cards;
        }
    }
}