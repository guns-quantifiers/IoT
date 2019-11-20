using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class Deal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<CardType> PlayerHand { get; set; } = new List<CardType>();
        public List<CardType> CroupierHand { get; set; } = new List<CardType>();
        public bool IsEnded { get; set; } = false;

        public void Deconstruct(out IReadOnlyList<CardType> playerHand, out IReadOnlyList<CardType> croupierHand)
        {
            playerHand = PlayerHand;
            croupierHand = CroupierHand;
        }
    }
}