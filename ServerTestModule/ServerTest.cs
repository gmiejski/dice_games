using System.Linq;
using CommonInterfacesModule;
using Moq;
using NUnit.Framework;
using ServerModule;
using System.Collections.Generic;

namespace ServerTestModule
{
    [TestFixture]
    public class ServerTest
    {

        private IServer _instance;
        private Dictionary<string, IGameController> _activeGames;
        private Dictionary<string, CreatedGame> _availableGames;
        private Dictionary<string, string> _loggedPlayers;
        private Mock<GameControllerFactory> _gameControllerFactoryMock;

        private const string OwnerName = "ownerName";
        private const string GameName = "gameName";
        private GameType GameType = GameType.NPlus;
        private const int NumberOfPlayers = 3;
        private const int NumberOfBots = 1;
        private BotLevel BotLevel = BotLevel.Easy;

        [SetUp]
        public void SetUp()
        {
            _activeGames = new Dictionary<string, IGameController>();
            _availableGames = new Dictionary<string, CreatedGame>();
            _loggedPlayers = new Dictionary<string, string>();
            _gameControllerFactoryMock = new Mock<GameControllerFactory>();

            _instance = new Server(_gameControllerFactoryMock.Object, _activeGames, _availableGames, _loggedPlayers);
        }

        [Test]
        public void ShouldRegisterPlayer()
        {
            string playerName = "playerTestName";
            string contextId = "context";

            Assert.True(_instance.RegisterPlayer(playerName, contextId));
            Assert.AreEqual(contextId,_loggedPlayers[playerName]);
        }

        [Test]
        public void ShouldNotregisterPlayerWhenEmptyArgumentString()
        {
            string playerName = "playerName";
            string contextId = "context";

            Assert.False(_instance.RegisterPlayer(playerName, ""));
            Assert.False(_loggedPlayers.ContainsKey(playerName));

            Assert.False(_instance.RegisterPlayer("", contextId));
            Assert.False(_loggedPlayers.ContainsKey(playerName));
        }

        [Test]
        public void ShouldNotRegisterPlayerWhenArgumentIsNull()
        {
            string contextId = "someContext";
            string playerName = "playerName";

            Assert.False(_instance.RegisterPlayer(null, contextId));
            Assert.False(_loggedPlayers.ContainsKey(playerName));

            Assert.False(_instance.RegisterPlayer(playerName, null));
            Assert.False(_loggedPlayers.ContainsKey(playerName));
        }

        [Test]
        public void ShouldNotRegisterPlayerWhenDesiredNameIsInUse()
        {
            string playerName = "playerName";
            string contextId = "context";
            string anotherContextId = "anotherContextId";

            Assert.True(_instance.RegisterPlayer(playerName, contextId));

            Assert.False(_instance.RegisterPlayer(playerName, anotherContextId));
            Assert.AreEqual(contextId, _loggedPlayers[playerName]);
        }

        [Test]
        public void ShouldCreateNewGame()
        {
            CreatedGame createdGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);

            Assert.NotNull(createdGame);
            Assert.AreEqual(createdGame.GameName, GameName);
            Assert.AreEqual(createdGame.BotLevel, BotLevel);
            Assert.AreEqual(createdGame.GameType, GameType);
            Assert.AreEqual(createdGame.NumberOfBots, NumberOfBots);
            Assert.AreEqual(createdGame.NumberOfPlayers, NumberOfPlayers);
            Assert.AreEqual(createdGame.OwnerName, OwnerName);
            Assert.True(createdGame.PlayerNames.Contains(OwnerName));
            Assert.AreEqual(createdGame,_availableGames[GameName]);
        }

        [Test]
        public void ShouldCreateNewGameWhenGameNameAlreadyInUse()
        {
            CreatedGame createdGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);
            CreatedGame secondGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);

            Assert.NotNull(createdGame);
            Assert.Null(secondGame);
            Assert.AreEqual(createdGame,_availableGames[GameName]);

        }

        [Test]
        public void ShouldReturnAvailableGames()
        {
            CreatedGame createdGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);
            List<CreatedGame> createdGames = _instance.GetAvailableGames();
            Assert.True(createdGames.Contains(createdGame));
        }

        [Test]
        public void ShouldAddPlayerToExistingCreatedGame()
        {
            string playerName = "playerName";
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            createdGameMock.Setup(cr => cr.AddPlayer(playerName)).Returns(true);
            _availableGames.Add(GameName,createdGameMock.Object);

            bool result = _instance.JoinGame(playerName, GameName);

            Assert.True(result);
            createdGameMock.Verify(cr => cr.AddPlayer(It.IsAny<string>()),Times.Once);
        }

        [Test]
        public void ShouldNotAddPlayerWhenNoCreatedGameFound()
        {
            string playerName = "playerName";

            bool result = _instance.JoinGame(playerName, GameName);

            Assert.False(result);
        }
        [Test]
        public void ShouldReturnFalseWhenUnnableToJoinPlayerToGame()
        {
            string playerName = "playerName";
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            createdGameMock.Setup(cr => cr.AddPlayer(playerName)).Returns(false);
            _availableGames.Add(GameName, createdGameMock.Object);

            bool result = _instance.JoinGame(playerName, GameName);

            Assert.False(result);
        }

        [Test]
        public void ShouldRemoveCreatedGameFromAvailableGamesWhenIsReadyToStart()
        {
            string playerName = "playerName";
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            createdGameMock.Setup(cr => cr.AddPlayer(playerName)).Returns(true);
            createdGameMock.Setup(cr => cr.IsReadyToStart()).Returns(true);
            _availableGames.Add(GameName, createdGameMock.Object);

            bool result = _instance.JoinGame(playerName, GameName);

            Assert.True(result);
            createdGameMock.Verify(cr => cr.AddPlayer(It.IsAny<string>()), Times.Once);
            createdGameMock.Verify(cr => cr.IsReadyToStart(), Times.Once);
            Assert.False(_availableGames.ContainsKey(GameName));
        }

        [Test]
        public void ShouldAddGameControllerToActiveGamesWhenLastPlayerSuccesfullyJoinedCreatedGame()
        {
            const string playerName = "playerName";

            var gameControllerMock = new Mock<IGameController>();

            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);

            createdGameMock.Setup(cr => cr.AddPlayer(playerName)).Returns(true);
            createdGameMock.Setup(cr => cr.IsReadyToStart()).Returns(true);
            _gameControllerFactoryMock.Setup(mock => mock.CreateGameController(createdGameMock.Object)).Returns(gameControllerMock.Object);

            _availableGames.Add(GameName, createdGameMock.Object);


            bool result = _instance.JoinGame(playerName, GameName);

            Assert.True(result);
            createdGameMock.Verify(cr => cr.AddPlayer(playerName), Times.Once);
            createdGameMock.Verify(cr => cr.IsReadyToStart(), Times.Once);
            _gameControllerFactoryMock.Verify(factory => factory.CreateGameController(It.IsAny<CreatedGame>()), Times.Once());
            Assert.True(_activeGames.ContainsKey(GameName));
            Assert.AreEqual(gameControllerMock.Object,_activeGames[GameName]);
        }
    }
}
