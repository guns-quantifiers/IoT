using Core.Components;
using Core.Constants;
using Core.Models;
using Strategies;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StrategyTests
{
    public class TestCaseGeneratorV2
    {
        public List<PlayerDecision> Generate(TestCaseSettings settings)
        {
            GameDeck deck = new GameDeck(settings.NumberOfDecks, settings.DeckPenetration, settings.Seed);
            IBetMultiplierCalculator betMultiplierCalculator = settings.CalculatorConfiguration.ToBetCalculator();
            IStrategyContext countingStrategy = settings.CountingStrategyModel.GetStrategyContext();
            IStrategyProvider strategy = new ChartedBasicStrategy();
            List<PlayerDecision> decisions = new List<PlayerDecision>();

            Game currentGame = new Game();
            int dealsCounter = -1, gamesCounter = 0;
            double currentBet, currentBetCounter, currentBetMultiplier;
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
                    default:
                        throw new ArgumentException("Unknown move strategy: " + move);
                }

                decisions.Add(new PlayerDecision(move, currentDecisionImpact, currentGame.Clone(), currentBetCounter, currentBetMultiplier));

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
                currentBetMultiplier = betMultiplierCalculator.Calculate(currentBetCounter).Value;
                currentBet = settings.MinimumBet;
                if (currentBetMultiplier > 0)
                {
                    currentBet += currentBetMultiplier * settings.BasicBet;
                    if (currentBet > settings.MaximumBet)
                    {
                        currentBet = settings.MaximumBet;
                    }
                }
                CroupierDraws();
                PlayerDraws();
                PlayerDraws();
            }
            void PlayerDraws() => currentDeal.PlayerHand.Cards.Add(deck.DrawNext());
            void CroupierDraws() => currentDeal.CroupierHand.Cards.Add(deck.DrawNext());
        }
    }
}
