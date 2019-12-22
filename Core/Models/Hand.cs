using Core.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Core.Models
{
    public class Hand
    {
        [BsonRepresentation(BsonType.String)]
        public List<CardType> Cards { get; set; }

        public Hand() { }

        public Hand(List<CardType> cards) => Cards = cards ?? new List<CardType>();
    }
}
