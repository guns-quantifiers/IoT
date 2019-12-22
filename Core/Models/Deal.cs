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
    }

    public struct DealId
    {
        public DealId(ObjectId value) => Value = value;

        public ObjectId Value { get; }

        public static DealId New() => new DealId(ObjectId.GenerateNewId());
        public static bool operator ==(DealId first, DealId second) => first.Value == second.Value;
        public static bool operator !=(DealId first, DealId second) => first.Value != second.Value;
    }
}