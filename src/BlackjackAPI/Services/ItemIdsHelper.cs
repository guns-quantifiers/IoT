using Core.Exceptions;
using Core.Models;
using MongoDB.Bson;

namespace BlackjackAPI.Services
{
    public static class ItemIdsHelper
    {
        public static DealId ToDealId(this string value) => new DealId(ToObjectId(value));
        public static GameId ToGameId(this string value) => new GameId(ToObjectId(value));

        private static ObjectId ToObjectId(this string value)
        {
            if (ObjectId.TryParse(value, out ObjectId id))
            {
                return id;
            }
            else
            {
                throw new BlackjackBadRequestException("Deal token should be a GUID value, got: " + value);
            }
        }
    }
}
