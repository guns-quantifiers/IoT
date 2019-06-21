using System;

namespace BlackjackAPI.Models
{
    public class Deal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Hand PlayerHand { get; set; }
        public Hand CroupierHand { get; set; }
    }
}
