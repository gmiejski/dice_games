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

        private string OwnerName = "ownerName";
        private string GameName = "gameName";
        private GameType GameType = GameType.NPlus;
        private int NumberOfPlayers = 3;
        private int NumberOfBots = 1;
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
        public void ShouldUnregisterPlayer()
        {
            string playerName = "playerName";
            string contextId = "context";
            _loggedPlayers.Add(playerName, contextId);
            Assert.True(_instance.UnregisterPlayer(playerName));
            Assert.True(_loggedPlayers.Count == 0);
        }

        [Test]
        public void ShouldNotUnregisterPlayerWhenArgumentIsNullOrEmpty()
        {
            string playerName = "playerName";
            string contextId = "context";
            _loggedPlayers.Add(playerName, contextId);
            Assert.False(_instance.UnregisterPlayer(null));
            Assert.True(_loggedPlayers.Keys.Contains(playerName));
            Assert.False(_instance.UnregisterPlayer(""));
            Assert.True(_loggedPlayers.Keys.Contains(playerName));
        }

        [Test]
        public void ShouldNotUnregisterPlayerWhenDesiredNameIsNotInUse()
        {
            string playerName = "playerName";
            string anotherPlayerName = "playerName2";
            string contextId = "context";
            _loggedPlayers.Add(playerName, contextId);
            Assert.False(_instance.UnregisterPlayer(anotherPlayerName));
            Assert.True(_loggedPlayers.Keys.Contains(playerName));
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
        public void ShouldNotCreateNewGameWhenGameNameAlreadyInUse()
        {
            CreatedGame createdGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);
            CreatedGame secondGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);

            Assert.NotNull(createdGame);
            Assert.Null(secondGame);
            Assert.AreEqual(createdGame,_availableGames[GameName]);

        }

        [Test]
        public void ShouldNotCreateNewGameWhenArgumentsAreMalformed()
        {
            Assert.Null(_instance.CreateGame(null, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame("", GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, null, GameType, NumberOfPlayers, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, "", GameType, NumberOfPlayers, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, GameName, GameType, -1, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, -1, BotLevel));
        }

        [Test]
        public void ShouldNotDeleteGameWhenArgumentIsNullOrEmpty()
        {
            Assert.False(_instance.DeleteGame(null));
            Assert.False(_instance.DeleteGame(""));
        }

        [Test]
        public void ShouldNotDeleteGameWhenGameNameIsNotInUse()
        {
            string firstGame = "firstGame";
            string secondGame = "secondGame";
            string thirdGame = "thirdGame";
            _availableGames.Add(firstGame, new Mock<CreatedGame>(null,null,null,null,null,null).Object);
            _activeGames.Add(secondGame, new Mock<IGameController>().Object);
            Assert.False(_instance.DeleteGame(thirdGame));
            Assert.True(_availableGames.Keys.Contains(firstGame));
            Assert.True(_activeGames.Keys.Contains(secondGame));
        }

        [Test]
        public void ShouldDeleteGameWhenGameIsAvailable()
        {
            _availableGames.Add(GameName, new Mock<CreatedGame>(null, null, null, null, null, null).Object);
            Assert.True(_instance.DeleteGame(GameName));
            Assert.False(_availableGames.Keys.Contains(GameName));
        }

        [Test]
        public void ShouldDeleteGameWhenGameIsActive()
        {
            _activeGames.Add(GameName, new Mock<IGameController>().Object);
            Assert.True(_instance.DeleteGame(GameName));
            Assert.False(_activeGames.Keys.Contains(GameName));
        }

        [Test]
        public void ShouldNotRemovePlayerWhenPlayerNameIsNullOrEmpty()
        {
            Assert.False(_instance.RemovePlayer(null));
            Assert.False(_instance.RemovePlayer(""));
        }

        [Test]
        public void ShouldNotRemovePlayerWhenPlayerNotInAvailableAndActiveGame()
        {
            string playerName = "playerName";
            string firstGame = "firstGame";
            string secondGame = "secondGame";
            var gameStateMock = new Mock<GameState>();
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            var gameControllerMock = new Mock<IGameController>();
            
            createdGameMock.SetupGet(cr => cr.PlayerNames).Returns(new List<string>());
            gameStateMock.SetupGet(g => g.PlayerStates).Returns(new Dictionary<string, PlayerState>());
            gameControllerMock.SetupGet(g => g.GameState).Returns(gameStateMock.Object);
            
            _availableGames.Add(firstGame, createdGameMock.Object);
            _activeGames.Add(secondGame, gameControllerMock.Object);
            
            Assert.False(_instance.RemovePlayer(playerName));
            createdGameMock.VerifyGet(cr => cr.PlayerNames, Times.Once);
            gameControllerMock.VerifyGet(g => g.GameState, Times.Once);
            gameStateMock.VerifyGet(g => g.PlayerStates, Times.Once);
        }

        [Test]
        public void ShouldRemovePlayerWhenPlayerInAvailableGame()
        {
            string playerName = "playerName";
            string anotherPlayerName = "anotherPlayer";
            List<string> playerNames = new List<string> { playerName, anotherPlayerName };
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);

            createdGameMock.SetupGet(cr => cr.PlayerNames).Returns(playerNames);

            _availableGames.Add(GameName, createdGameMock.Object);

            Assert.True(_instance.RemovePlayer(playerName));
            createdGameMock.VerifyGet(cr => cr.PlayerNames, Times.Exactly(3));
            Assert.False(_availableGames.First().Value.PlayerNames.Contains(playerName));
            Assert.True(_availableGames.First().Value.PlayerNames.Contains(anotherPlayerName));
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
            string contextId = "contextId";

            _loggedPlayers.Add(playerName, contextId);
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
            string contextId = "contextId";

            _loggedPlayers.Add(playerName, contextId);
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            var gameControllerMock = new Mock<IGameController>();
            createdGameMock.Setup(cr => cr.AddPlayer(playerName)).Returns(true);
            createdGameMock.Setup(cr => cr.IsReadyToStart()).Returns(true);
            _gameControllerFactoryMock.Setup(gc => gc.CreateGameController(createdGameMock.Object)).Returns(gameControllerMock.Object);
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
            string playerName = "playerName";
            string contextId = "contextId";

            _loggedPlayers.Add(playerName, contextId);
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

        [Test]
        public void ShouldReturnNullWhenAskingForGameStateOfNotExistingGameController()
        {

            string gameName = "gameName";

            GameState gameState = _instance.GetGameState(gameName);

            Assert.Null(gameState);

        }

        [Test]
        public void ShouldReturnGameStateOfExistingGameController()
        {
            var gameController = new Mock<IGameController>();
            var gameState = new Mock<GameState>();
            gameController.Setup(gc => gc.GameState).Returns(gameState.Object);

            string gameName = "gameName";
            _activeGames.Add(gameName, gameController.Object);

            var result = _instance.GetGameState(gameName);

            Assert.NotNull(result);
            Assert.AreEqual(result, gameState.Object);

        }

        [Test]
        public void ShouldReturnFalseWhenMakingMoveWithNullParameters()
        {
            string playerName = "playerName";
            string gameName = "gameName";
            var move = new Mock<Move>(null);

            Assert.False(_instance.MakeMove(null,gameName,move.Object));
            Assert.False(_instance.MakeMove(playerName,null,move.Object));
            Assert.False(_instance.MakeMove(playerName,gameName,null));
        }

        [Test]
        public void ShouldReturnFalseWhenMakingMoveWithEmptyStringParameters()
        {
            string playerName = "playerName";
            string gameName = "gameName";
            var move = new Mock<Move>(null);

            Assert.False(_instance.MakeMove("", gameName, move.Object));
            Assert.False(_instance.MakeMove(playerName, "", move.Object));
        }

        [Test]
        public void ShouldReturnFalseWhenMakingMoveAndNoGameControllerFound()
        {
            string playerName = "playerName";
            string gameName = "gameName";
            var move = new Mock<Move>(null);

            bool result = _instance.MakeMove(playerName, gameName, move.Object);

            Assert.False(result);
        }

        [Test]
        public void ShouldSendMoveToMathingGameController()
        {
            var gameController = new Mock<IGameController>();
            
            var move = new Mock<Move>(null);
            var gameName = "gameName";
            var playerName = "playerName";

            gameController.Setup(gc => gc.MakeMove(playerName, move.Object)).Returns(true);
            _activeGames.Add(gameName, gameController.Object);

            bool result = _instance.MakeMove(playerName,gameName,move.Object);

            Assert.True(result);
            gameController.Verify(gc => gc.MakeMove(playerName,move.Object),Times.Once);

        }

    }
}
