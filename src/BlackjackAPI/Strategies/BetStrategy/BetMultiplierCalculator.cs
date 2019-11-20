using BlackjackAPI.Exceptions;
using BlackjackAPI.Models;
using System;

namespace BlackjackAPI.Strategies.BetStrategy
{
    public class BetMultiplierCalculator
    {
        public BetMultiplier Calculate(Game game, Guid dealId)
        {
            var deal = game.History.Find(d => d.Id == dealId);
            if (deal == null)
            {
                throw new NotFoundException($"Specified deal: {dealId} does not belong to game: {game.Id}.");
            }

            return MultiplierFunction(game.CardCounter + game.DealCardCounter.Count(deal));
        }

        private BetMultiplier MultiplierFunction(int counter)
        => new BetMultiplier()
        {
            Value = 2.0 / (1 + Math.Exp(-0.098 * counter))
        };
    }
}
