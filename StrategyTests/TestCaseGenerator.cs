using Core.Components;
using Core.Constants;
using Core.Models;
using Strategies;
using Strategies.BetStrategy.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StrategyTests
{
    public class TestCaseGenerator
    {
        public List<PlayerDecision> Generate(TestCaseSettings settings)
        {
            GameDeck deck = new GameDeck(settings.NumberOfDecks, settings.DeckPenetration, settings.Seed);
            IBetMultiplierCalculator betMultiplierCalculator = settings.CalculatorConfiguration.ToBetCalculator();
            IStrategyContext countingStrategy = settings.CountingStrategyModel.GetStrategyContext();
            IStrategyProvider strategy = new ChartedBasicStrategy();
            List<PlayerDecision> decisions = new List<PlayerDecision>();

            Game currentGame = new Game();
            int dealsCounter = -1, currentBetCounter, gamesCounter = 0;
            double currentBet;
            Deal currentDeal;
            StartNewDeal();
            while (gamesCounter < settings.GamesToGenerate)
            {
                double currentDecisionImpact = 0;

                DrawStrategy move = strategy.Get(currentGame, currentDeal);
                switch (move)
                {
                    case DrawStrategy.Hit:
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
                        EndDeal();
                        break;
                    case DrawStrategy.None:
                    default:
                        throw new ArgumentException("Unknown move strategy: " + move);
                }

                decisions.Add(new PlayerDecision(move, currentDecisionImpact, currentGame.Clone(), currentBetCounter, betMultiplierCalculator.Calculate(currentBetCounter).Value));

                if (currentDeal.IsEnded)
                {
                    StartNewDeal();
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
                    gamesCounter++;
                }
                currentDeal = currentGame.NewDeal();
                Debug.WriteLine($"Started new deal ({dealsCounter}): " + currentDeal.Id.Value.Increment);
                dealsCounter++;
                currentBetCounter = countingStrategy.GetCounter(currentGame, currentDeal);
                currentBet = settings.MinimumBet * betMultiplierCalculator.Calculate(currentBetCounter).Value;
                CroupierDraws();
                PlayerDraws();
                PlayerDraws();
            }
            void PlayerDraws() => currentDeal.PlayerHand.Cards.Add(deck.DrawNext());
            void CroupierDraws() => currentDeal.CroupierHand.Cards.Add(deck.DrawNext());
        }
    }

    public class TestCaseSettings
    {
        public TestCaseSettings(int decks, SetCountingStrategyModel countingStrategyModel, int minimumBet, double maxDeckPenetration, int gamesToGenerate, ICalculatorConfiguration calculatorConfiguration, int seed)
        {
            NumberOfDecks = decks;
            CountingStrategyModel = countingStrategyModel;
            MinimumBet = minimumBet;
            DeckPenetration = maxDeckPenetration;
            GamesToGenerate = gamesToGenerate;
            CalculatorConfiguration = calculatorConfiguration;
            Seed = seed;
        }

        public int GamesToGenerate { get; }
        public int Seed { get; }
        public ICalculatorConfiguration CalculatorConfiguration { get; }
        public int NumberOfDecks { get; }
        public SetCountingStrategyModel CountingStrategyModel { get; }
        public int MinimumBet { get; }
        public double DeckPenetration { get; }
    }
}
