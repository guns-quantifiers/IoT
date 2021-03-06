﻿using Core.Constants;
using Core.Models;

namespace StrategyTests
{
    public class PlayerDecision
    {
        public PlayerDecision(DrawStrategy type, double value, Game gameSnapshot, int counter, double betMultiplier)
        {
            Type = type;
            Value = value;
            GameSnapshot = gameSnapshot;
            Counter = counter;
            BetMultiplier = betMultiplier;
        }

        public DrawStrategy Type { get; }
        public double Value { get; }
        public double BetMultiplier { get; }
        public int Counter { get; }
        public Game GameSnapshot { get; }
    }
}