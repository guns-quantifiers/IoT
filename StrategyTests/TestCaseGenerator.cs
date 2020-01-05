using Core.Components;
using Core.Constants;
using Core.Models;
using Strategies;
using Strategies.BetStrategy;
using Strategies.StrategyContexts.Knockout;
using Strategies.StrategyContexts.SilverFox;
using Strategies.StrategyContexts.UstonSS;
using System;
using System.Collections.Generic;

namespace StrategyTests
{
    public class TestCaseGenerator
    {
        public List<PlayerDecision> Generate(TestCaseSettings settings)
        {
            GameDeck deck = new GameDeck(settings.Decks, settings.MaxDeckPenetration);
            IStrategyContext countingStrategy = GetStrategyContext(settings);
            IBetMultiplierCalculator betCalculator = new BetMultiplierCalculator();
            IStrategyProvider strategy = new ChartedBasicStrategy();
            List<PlayerDecision> decisions = new List<PlayerDecision>(settings.DecisionsAmount);
            Game currentGame = new Game();
            double currentBet;
            Deal currentDeal;
            InitDeal();
            for (int decisionNumber = 0; decisionNumber < settings.DecisionsAmount; decisionNumber++, ShuffleIfNecessary())
            {
                double currentDecisionImpact = 0;
                if (currentDeal.IsEnded)
                {
                    InitDeal();
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

                decisions.Add(new PlayerDecision(move, currentDecisionImpact, currentGame.Clone()));

                void EndDeal()
                {
                    if (currentDeal.PlayerHand.Cards.Sum() > 21)
                    {
                        currentDecisionImpact = -currentBet;
                    }
                    else
                    {
                        while (currentDeal.CroupierHand.Cards.Sum() < 17)
                        {
                            CroupierDraws();
                        }

                        int croupierSum = currentDeal.CroupierHand.Cards.Sum();
                        if (croupierSum > 21)
                        {
                            currentDecisionImpact = currentBet;
                        }
                        else
                        {
                            int playerSum = currentDeal.PlayerHand.Cards.Sum();
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

            void InitDeal()
            {
                currentDeal = currentGame.NewDeal();
                currentBet = settings.MinimumBet * betCalculator.Calculate(countingStrategy.GetCounter(currentGame, currentDeal)).Value;
                CroupierDraws();
                PlayerDraws();
                PlayerDraws();
            }
            void PlayerDraws() => currentDeal.PlayerHand.Cards.Add(deck.DrawNext());
            void CroupierDraws() => currentDeal.CroupierHand.Cards.Add(deck.DrawNext());
            void ShuffleIfNecessary()
            {
                if (deck.CheckShuffle())
                {
                    currentGame = new Game();
                }
            }
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
        public TestCaseSettings(int decisionsAmount, int decks, CountingStrategy countingStrategy, int minimumBet, float maxDeckPenetration)
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
        public float MaxDeckPenetration { get; }
    }
}
