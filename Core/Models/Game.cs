using Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class Game
    {
        public GameId Id { get; set; } = GameId.New();
        public List<Deal> History { get; set; } = new List<Deal>();

        public Deal NewDeal()
        {
            var newDeal = new Deal();
            History.Add(newDeal);
            return newDeal;
        }

        public void EndDeal(DealId dealId)
        {
            var deal = History.Find(d => d.Id == dealId);
            if (deal.IsEnded)
            {
                throw new DealEndedException($"Cannot end already ended deal: {dealId.ToString()}");
            }
            deal.IsEnded = true;
        }

        public bool TryGetDeal(DealId id, out Deal deal)
        {
            deal = History.SingleOrDefault(d => d.Id == id);
            return deal != null;
        }
    }

    public struct GameId
    {
        public GameId(Guid value) => Value = value;

        public Guid Value { get; }

        public static GameId New() => new GameId(Guid.NewGuid());

        public static bool operator ==(GameId first, GameId second) => first.Value == second.Value;
        public static bool operator !=(GameId first, GameId second) => first.Value != second.Value;
    }
}
