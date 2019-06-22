using BlackjackAPI.Models;
using BlackjackAPI.Services;
using BlackjackAPI.Strategies.BetStrategy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace CardCountingTests
{
    [TestFixture]
    public class CardCountingTest
    {
        private IGameContext _gameContext;
        private Mock<IGameSaver> _gameSaverMock = new Mock<IGameSaver>();
        private Mock<ILogger<UstonSSGameContext>> _loggerMock = new Mock<ILogger<UstonSSGameContext>>();
        private BetMultiplierCalculator _betMultiplierCalculator;

        [SetUp]
        public void Init()
        {
            _betMultiplierCalculator = new BetMultiplierCalculator();
            WithGameSaver();
            WithLogger();
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

        private void WithLogger()
        {
            //_loggerMock.Setup(s => s.LogWarning(It.IsAny<string>()));
        }
        
        [Test]
        public void EmptyGame()
        {
            var newGame = _gameContext.NewGame();
            ThenCounterIs(newGame, -6);
        }

        [Test]
        public void GameWithOneOpen()
        {
            var newGame = _gameContext.NewGame();
            var newDeal = newGame.NewDeal();
            ThenCounterIs(newGame, newDeal, - 6);
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
            newDeal.PlayerHand = new List<CardType>(){card};
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
