using Core.Components;
using Core.Constants;
using Core.Models;
using Strategies;
using Strategies.StrategyContexts.Knockout;
using Strategies.StrategyContexts.SilverFox;
using Strategies.StrategyContexts.UstonSS;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StrategyTests
{
    public class TestCaseGenerator
    {
        private readonly IBetMultiplierCalculator _betMultiplierCalculator;

        public TestCaseGenerator(IBetMultiplierCalculator betMultiplierCalculator)
        {
            _betMultiplierCalculator = betMultiplierCalculator;
        }

        public List<PlayerDecision> Generate(TestCaseSettings settings)
        {
            GameDeck deck = new GameDeck(settings.Decks, settings.MaxDeckPenetration);
            IStrategyContext countingStrategy = GetStrategyContext(settings);
            IStrategyProvider strategy = new ChartedBasicStrategy();
            List<PlayerDecision> decisions = new List<PlayerDecision>(settings.DecisionsAmount);

            Game currentGame = new Game();
            int dealsCounter = -1;
            double currentBet;
            int currentBetCounter;
            Deal currentDeal;
            StartNewDeal();
            for (int decisionNumber = 0; decisionNumber < settings.DecisionsAmount; decisionNumber++)
            {
                double currentDecisionImpact = 0;
                if (currentDeal.IsEnded)
                {
                    StartNewDeal();
                }

                DrawStrategy move = strategy.Get(currentGame, currentDeal);
                switch (move)
                {
                    case DrawStrategy.Hit:
                        PlayerDraws();
                        break;

                    case DrawStrategy.DoubleDown:
                        currentBet *= 2;
                        PlayerDraws();
                        break;
                    case DrawStrategy.DoubleDownOrHit:
                        if (currentDeal.PlayerHand.Cards.Count == 2)
                            currentBet *= 2;
                        PlayerDraws();
                        break;

                    case DrawStrategy.DoubleDownOrStand:
                        if (currentDeal.PlayerHand.Cards.Count > 2)
                        {
                            EndDeal();
                            break;
                        }
                        PlayerDraws();
                        currentBet *= 2;
                        break;

                    case DrawStrategy.Stand:
                    case DrawStrategy.None:
                        EndDeal();
                        break;
                    default:
                        throw new ArgumentException("Unknown move strategy: " + move);
                }

                if (dealsCounter % 2 == 0) // count only half of the deals to simulate other players existence in the same game
                {
                    decisions.Add(new PlayerDecision(move, currentDecisionImpact, currentGame.Clone(), currentBetCounter));
                }

                void EndDeal()
                {
                    if (currentDeal.PlayerHand.Cards.SafeSum() > 21)
                    {
                        currentDecisionImpact = -currentBet;
                    }
                    else
                    {
                        while (currentDeal.CroupierHand.Cards.Sum() < 17)
                        {
                            CroupierDraws();
                        }

                        int croupierSum = currentDeal.CroupierHand.Cards.SafeSum();
                        if (croupierSum > 21)
                        {
                            currentDecisionImpact = currentBet;
                        }
                        else
                        {
                            int playerSum = currentDeal.PlayerHand.Cards.SafeSum();
                            if (croupierSum > playerSum)
                            {
                                currentDecisionImpact = -currentBet;
                            }
                            else if (croupierSum == playerSum)
                            {
                                currentDecisionImpact = 0;
                            }
                            else
                            {
                                currentDecisionImpact = currentBet;
                            }

                        }
                    }
                    currentDeal.IsEnded = true;
                }
            }

            return decisions;

            void StartNewDeal()
            {
                if (deck.CheckShuffle())
                {
                    currentGame = new Game();
                }
                currentDeal = currentGame.NewDeal();
                Debug.WriteLine($"Started new deal ({dealsCounter}): " + currentDeal.Id.Value.Increment);
                dealsCounter++;
                currentBetCounter = countingStrategy.GetCounter(currentGame, currentDeal);
                currentBet = settings.MinimumBet * _betMultiplierCalculator.Calculate(currentBetCounter).Value;
                CroupierDraws();
                PlayerDraws();
                PlayerDraws();
            }
            void PlayerDraws() => currentDeal.PlayerHand.Cards.Add(deck.DrawNext());
            void CroupierDraws() => currentDeal.CroupierHand.Cards.Add(deck.DrawNext());
        }

        private IStrategyContext GetStrategyContext(TestCaseSettings settings) =>
            settings.CountingStrategy switch
            {
                CountingStrategy.UstonSS => new UstonSSStrategyContext(settings.Decks) as IStrategyContext,
                CountingStrategy.SilverFox => new SilverFoxStrategyContext(),
                CountingStrategy.Knockout => new KnockoutStrategyContext(),
                _ => throw new ArgumentException("Unknown counting strategy: " + settings.CountingStrategy)
            };
    }

    public class TestCaseSettings
    {
        public TestCaseSettings(int decisionsAmount, int decks, CountingStrategy countingStrategy, int minimumBet, double maxDeckPenetration)
        {
            DecisionsAmount = decisionsAmount;
            Decks = decks;
            CountingStrategy = countingStrategy;
            MinimumBet = minimumBet;
            MaxDeckPenetration = maxDeckPenetration;
        }

        public int DecisionsAmount { get; }
        public int Decks { get; }
        public CountingStrategy CountingStrategy { get; }
        public int MinimumBet { get; }
        public double MaxDeckPenetration { get; }
    }
}
