using Core.Components;
using Core.Constants;
using Core.Models;
using FluentAssertions;
using NUnit.Framework;
using Strategies.StrategyContexts.UstonSS;
using System.Collections.Generic;

namespace CardCountingTests
{
    [TestFixture]
    public class CardCountingTest
    {
        private IStrategyContext _strategyContext;
        private const int _deckAmount = 3;
        private const int _expectedBaseCounter = -4 * _deckAmount;

        [SetUp]
        public void Init()
        {
            _strategyContext = new UstonSSStrategyContext(_deckAmount);
        }


        [Test]
        public void GameWithOneOpen()
        {
            var newGame = new Game();
            var newDeal = newGame.NewDeal();
            ThenCounterIs(newGame, newDeal, _expectedBaseCounter);
        }

        [TestCase(CardType.Two, 2)]
        [TestCase(CardType.Three, 2)]
        [TestCase(CardType.Four, 2)]
        [TestCase(CardType.Five, 3)]
        [TestCase(CardType.Six, 2)]
        [TestCase(CardType.Seven, 1)]
        [TestCase(CardType.Eight, 0)]
        [TestCase(CardType.Nine, -1)]
        [TestCase(CardType.Ten, -2)]
        [TestCase(CardType.Jack, -2)]
        [TestCase(CardType.Queen, -2)]
        [TestCase(CardType.King, -2)]
        [TestCase(CardType.Ace, -2)]
        public void GameWithOneOpenAndCard(CardType card, int cardModifier)
        {
            var newGame = new Game();
            var newDeal = newGame.NewDeal();
            newDeal.PlayerHand.Cards = new List<CardType>() { card };
            ThenCounterIs(newGame, newDeal, _expectedBaseCounter + cardModifier);
        }

        private void ThenCounterIs(Game game, Deal deal, int correctCounter)
        {
            _strategyContext.GetCounter(game, deal).Should().Be(correctCounter);
        }
    }
}
