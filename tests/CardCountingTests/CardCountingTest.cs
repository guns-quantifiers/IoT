using System.Collections.Generic;
using Core.Components;
using Core.Constants;
using Core.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Strategies.GameContexts;

namespace CardCountingTests
{
    [TestFixture]
    public class CardCountingTest
    {
        private IGameContext _gameContext;
        private Mock<IGameSaver> _gameSaverMock = new Mock<IGameSaver>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>(MockBehavior.Loose);

        [SetUp]
        public void Init()
        {
            WithGameSaver();
            _gameContext = new UstonSSGameContext(_gameSaverMock.Object, _loggerMock.Object, 3);
            _gameContext.Initialize();
        }

        private void WithGameSaver()
        {
            _gameSaverMock.Setup(s => s.SaveGames(It.IsAny<List<Game>>()));
            _gameSaverMock.Setup(s => s.LoadGames())
                .Returns(new List<Game>()
                {

                });
        }

        [Test]
        public void EmptyGame()
        {
            var newGame = _gameContext.NewGame();
            var newDeal = newGame.NewDeal();
            ThenCounterIs(newGame, newDeal, -6);
        }

        [Test]
        public void GameWithOneOpen()
        {
            var newGame = _gameContext.NewGame();
            var newDeal = newGame.NewDeal();
            ThenCounterIs(newGame, newDeal, -6);
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
            var newGame = _gameContext.NewGame();
            var newDeal = newGame.NewDeal();
            newDeal.PlayerHand = new List<CardType>() { card };
            ThenCounterIs(newGame, -6);
            ThenCounterIs(newGame, newDeal, -6 + cardModifier);
        }

        private void ThenCounterIs(Game game, int correctCounter)
        {
            game.CardCounter.Should().Be(correctCounter);
        }

        private void ThenCounterIs(Game game, Deal deal, int correctCounter)
        {
            (game.CardCounter +
            game.DealCardCounter.Count(deal))
                    .Should().Be(correctCounter);
        }
    }
}
