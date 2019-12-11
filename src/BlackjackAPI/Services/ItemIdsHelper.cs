using System;
using Core.Exceptions;
using Core.Models;

namespace BlackjackAPI.Services
{
    public static class ItemIdsHelper
    {
        public static DealId ToDealId(this string value) => new DealId(ToGuid(value));
        public static GameId ToGameId(this string value) => new GameId(ToGuid(value));

        private static Guid ToGuid(this string value)
        {
            if (Guid.TryParse(value, out Guid guid))
            {
                return guid;
            }
            else
            {
                throw new BlackjackBadRequestException("Deal token should be a GUID value, got: " + value);
            }
        }
    }
}
