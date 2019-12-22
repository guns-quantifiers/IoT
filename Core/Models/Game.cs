using Core.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class Game
    {
        [BsonId]
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

    public class GameId
    {
        public GameId(ObjectId value) => Value = value;
        public GameId() { }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Value { get; set; }

        public override string ToString() => Value.ToString();

        protected bool Equals(GameId other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GameId)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static GameId New() => new GameId(ObjectId.GenerateNewId());

        public static bool operator ==(GameId first, GameId second) => first.Value == second.Value;
        public static bool operator !=(GameId first, GameId second) => first.Value != second.Value;
    }
}
