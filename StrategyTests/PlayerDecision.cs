using Core.Constants;
using Core.Models;

namespace StrategyTests
{
    public class PlayerDecision
    {
        public PlayerDecision(DrawStrategy type, double value, Game gameSnapshot, int counter)
        {
            Type = type;
            Value = value;
            GameSnapshot = gameSnapshot;
            Counter = counter;
        }

        public DrawStrategy Type { get; }
        public double Value { get; }
        public int Counter { get; }
        public Game GameSnapshot { get; }
    }
}