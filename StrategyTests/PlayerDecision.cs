using Core.Constants;
using Core.Models;

namespace StrategyTests
{
    public class PlayerDecision
    {
        public PlayerDecision(DrawStrategy type, double value, Game gameSnapshot)
        {
            Type = type;
            Value = value;
            GameSnapshot = gameSnapshot;
        }

        public DrawStrategy Type { get; }
        public double Value { get; }
        public Game GameSnapshot { get; }
    }
}