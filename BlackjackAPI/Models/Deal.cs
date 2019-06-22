using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class Deal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<CardType> PlayerHand { get; set; }
        public List<CardType> CroupierHand { get; set; }
        public bool IsEnded { get; set; }
    }
}