using Core.Constants;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Core.Models
{
    public class Deal
    {
        public DealId Id { get; set; } = DealId.New();
        public Hand PlayerHand { get; set; } = new Hand(new List<CardType>());
        public Hand CroupierHand { get; set; } = new Hand(new List<CardType>());
        public bool IsEnded { get; set; } = false;

        public void Deconstruct(out IReadOnlyList<CardType> playerHand, out IReadOnlyList<CardType> croupierHand)
        {
            playerHand = PlayerHand.Cards;
            croupierHand = CroupierHand.Cards;
        }

        public Deal Clone()
        {
            return new Deal
            {
                Id = Id,
                PlayerHand = new Hand(new List<CardType>(PlayerHand.Cards)),
                CroupierHand = new Hand(new List<CardType>(CroupierHand.Cards)),
                IsEnded = IsEnded
            };
        }
    }

    public class DealId
    {
        public DealId(ObjectId value) => Value = value;
        public DealId() { }
        public ObjectId Value { get; set; }

        public override string ToString() => Value.ToString();

        protected bool Equals(DealId other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DealId)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static DealId New() => new DealId(ObjectId.GenerateNewId());
        public static bool operator ==(DealId first, DealId second) => first.Value == second.Value;
        public static bool operator !=(DealId first, DealId second) => first.Value != second.Value;
    }
}