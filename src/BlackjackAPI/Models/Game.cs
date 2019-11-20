using BlackjackAPI.Exceptions;
using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<Deal> History { get; set; } = new List<Deal>();
        /// <summary>
        /// Counter contains sum from all already ended deals.
        /// Deals that are not ended, should be taken into account separately.
        /// </summary>
        public int CardCounter { get; set; } = -4;

        public IDealCardCounter DealCardCounter { get; set; }

        public Deal NewDeal()
        {
            var newDeal = new Deal();
            History.Add(newDeal);
            return newDeal;
        }

        public void EndDeal(Guid dealId)
        {
            var deal = History.Find(d => d.Id == dealId);
            if (deal.IsEnded)
            {
                throw new DealEndedException($"Cannot end already ended deal: {dealId.ToString()}");
            }
            CardCounter += DealCardCounter.Count(deal);
            deal.IsEnded = true;
        }
    }
}
