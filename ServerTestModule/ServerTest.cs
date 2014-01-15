using System;
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
        private const string PlayerName = "PlayerName";
        private const string ContextId = "playerContextId";
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

            Assert.True(_instance.RegisterPlayer(PlayerName, ContextId));
            Assert.AreEqual(ContextId, _loggedPlayers[PlayerName]);
        }

        [Test]
        public void ShouldNotregisterPlayerWhenEmptyArgumentString()
        {
            Assert.False(_instance.RegisterPlayer(PlayerName, string.Empty));
            Assert.False(_loggedPlayers.ContainsKey(PlayerName));

            Assert.False(_instance.RegisterPlayer(string.Empty, ContextId));
            Assert.False(_loggedPlayers.ContainsKey(PlayerName));
        }

        [Test]
        public void ShouldNotRegisterPlayerWhenArgumentIsNull()
        {
            Assert.False(_instance.RegisterPlayer(null, ContextId));
            Assert.False(_loggedPlayers.ContainsKey(PlayerName));

            Assert.False(_instance.RegisterPlayer(PlayerName, null));
            Assert.False(_loggedPlayers.ContainsKey(PlayerName));
        }

        [Test]
        public void ShouldNotRegisterPlayerWhenDesiredNameIsInUse()
        {
            var anotherContextId = "anotherContextId";

            Assert.True(_instance.RegisterPlayer(PlayerName, ContextId));

            Assert.False(_instance.RegisterPlayer(PlayerName, anotherContextId));
            Assert.AreEqual(ContextId, _loggedPlayers[PlayerName]);
        }

        [Test]
        public void ShouldUnregisterPlayer()
        {
            _loggedPlayers.Add(PlayerName, ContextId);
            Assert.True(_instance.UnregisterPlayer(PlayerName));
            Assert.True(_loggedPlayers.Count == 0);
        }

        [Test]
        public void ShouldNotUnregisterPlayerWhenArgumentIsNullOrEmpty()
        {
            _loggedPlayers.Add(PlayerName, ContextId);
            Assert.False(_instance.UnregisterPlayer(null));
            Assert.True(_loggedPlayers.Keys.Contains(PlayerName));
            Assert.False(_instance.UnregisterPlayer(string.Empty));
            Assert.True(_loggedPlayers.Keys.Contains(PlayerName));
        }

        [Test]
        public void ShouldNotUnregisterPlayerWhenDesiredNameIsNotInUse()
        {
            string anotherPlayerName = "PlayerName2";

            _loggedPlayers.Add(PlayerName, ContextId);

            Assert.False(_instance.UnregisterPlayer(anotherPlayerName));
            Assert.True(_loggedPlayers.Keys.Contains(PlayerName));
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
            Assert.Null(_instance.CreateGame(string.Empty, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, null, GameType, NumberOfPlayers, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, string.Empty, GameType, NumberOfPlayers, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, GameName, GameType, -1, NumberOfBots, BotLevel));
            Assert.Null(_instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, -1, BotLevel));
        }

        [Test]
        public void ShouldNotCreateGameWhenActiveGameWithSameNameExists()
        {
            var gameControllerMock = new Mock<IGameController>();

            _activeGames.Add(GameName,gameControllerMock.Object);
            Assert.Null(_instance.CreateGame(PlayerName,GameName, GameType,0,0,BotLevel));

        }

        [Test]
        public void ShouldNotDeleteGameWhenArgumentIsNullOrEmpty()
        {
            Assert.False(_instance.DeleteGame(null));
            Assert.False(_instance.DeleteGame(string.Empty));
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
            
            Assert.False(_instance.RemovePlayer(PlayerName));
            createdGameMock.VerifyGet(cr => cr.PlayerNames, Times.Once);
            gameControllerMock.VerifyGet(g => g.GameState, Times.Once);
            gameStateMock.VerifyGet(g => g.PlayerStates, Times.Once);
        }

        [Test]
        public void ShouldRemovePlayerWhenPlayerInAvailableGame()
        {
            string anotherPlayerName = "anotherPlayer";
            var PlayerNames = new List<string> { PlayerName, anotherPlayerName };
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);

            createdGameMock.SetupGet(cr => cr.PlayerNames).Returns(PlayerNames);

            _availableGames.Add(GameName, createdGameMock.Object);

            Assert.True(_instance.RemovePlayer(PlayerName));
            createdGameMock.VerifyGet(cr => cr.PlayerNames, Times.Exactly(3));
            Assert.False(_availableGames.First().Value.PlayerNames.Contains(PlayerName));
            Assert.True(_availableGames.First().Value.PlayerNames.Contains(anotherPlayerName));
        }

        [Test]
        public void ShouldRemoveAvailableGameWhenLastPlayerRemoved()
        {
            var PlayerNames = new List<string> { PlayerName };
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);

            createdGameMock.SetupGet(cr => cr.PlayerNames).Returns(PlayerNames);

            _availableGames.Add(GameName, createdGameMock.Object);

            Assert.True(_instance.RemovePlayer(PlayerName));
            createdGameMock.VerifyGet(cr => cr.PlayerNames, Times.Exactly(2));
            Assert.True(_availableGames.Count == 0);
        }

        [Test]
        public void ShouldRemovePlayerAndGameWhenPlayerInActiveGame()
        {
            var playerStates = new Dictionary<string, PlayerState> { {PlayerName, new Mock<PlayerState>(null).Object} };
            var gameStateMock = new Mock<GameState>();
            var gameControllerMock = new Mock<IGameController>();

            gameStateMock.SetupGet(g => g.PlayerStates).Returns(playerStates);
            gameControllerMock.SetupGet(g => g.GameState).Returns(gameStateMock.Object);

            _activeGames.Add(GameName, gameControllerMock.Object);

            Assert.True(_instance.RemovePlayer(PlayerName));
            gameStateMock.VerifyGet(g => g.PlayerStates, Times.Once);
            gameControllerMock.VerifyGet(g => g.GameState, Times.Once);
            Assert.True(_activeGames.Count == 0);
        }

        [Test]
        public void ShouldReturnAvailableGames()
        {
            var createdGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);
            var createdGames = _instance.GetAvailableGames();
            Assert.True(createdGames.Contains(createdGame));
        }

        [Test]
        public void ShouldReturnFalseWhenJoiningGameWithEmptyOrNullArguments()
        {
            Assert.False(_instance.JoinGame(string.Empty, GameName));
            Assert.False(_instance.JoinGame(PlayerName, string.Empty));
            Assert.False(_instance.JoinGame(PlayerName, string.Empty));
            Assert.False(_instance.JoinGame(null, GameName));
        }

        [Test]
        public void ShouldAddPlayerToExistingCreatedGame()
        {

            _loggedPlayers.Add(PlayerName, ContextId);
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            createdGameMock.Setup(cr => cr.AddPlayer(PlayerName)).Returns(true);
            _availableGames.Add(GameName,createdGameMock.Object);

            bool result = _instance.JoinGame(PlayerName, GameName);

            Assert.True(result);
            createdGameMock.Verify(cr => cr.AddPlayer(It.IsAny<string>()),Times.Once);
        }

        [Test]
        public void ShouldNotAddPlayerWhenNoCreatedGameFound()
        {
            bool result = _instance.JoinGame(PlayerName, GameName);

            Assert.False(result);
        }
        [Test]
        public void ShouldReturnFalseWhenUnnableToJoinPlayerToGame()
        {
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            createdGameMock.Setup(cr => cr.AddPlayer(PlayerName)).Returns(false);
            _availableGames.Add(GameName, createdGameMock.Object);

            bool result = _instance.JoinGame(PlayerName, GameName);

            Assert.False(result);
        }

        [Test]
        public void ShouldRemoveCreatedGameFromAvailableGamesWhenIsReadyToStart()
        {

            _loggedPlayers.Add(PlayerName, ContextId);
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);
            var gameControllerMock = new Mock<IGameController>();
            createdGameMock.Setup(cr => cr.AddPlayer(PlayerName)).Returns(true);
            createdGameMock.Setup(cr => cr.IsReadyToStart()).Returns(true);
            _gameControllerFactoryMock.Setup(gc => gc.CreateGameController(createdGameMock.Object)).Returns(gameControllerMock.Object);
            _availableGames.Add(GameName, createdGameMock.Object);

            bool result = _instance.JoinGame(PlayerName, GameName);

            Assert.True(result);
            createdGameMock.Verify(cr => cr.AddPlayer(It.IsAny<string>()), Times.Once);
            createdGameMock.Verify(cr => cr.IsReadyToStart(), Times.Once);
            Assert.False(_availableGames.ContainsKey(GameName));
        }

        [Test]
        public void ShouldAddGameControllerToActiveGamesWhenLastPlayerSuccesfullyJoinedCreatedGame()
        {

            _loggedPlayers.Add(PlayerName, ContextId);
            var gameControllerMock = new Mock<IGameController>();
            var createdGameMock = new Mock<CreatedGame>(null, null, null, null, null, null);

            createdGameMock.Setup(cr => cr.AddPlayer(PlayerName)).Returns(true);
            createdGameMock.Setup(cr => cr.IsReadyToStart()).Returns(true);
            _gameControllerFactoryMock.Setup(mock => mock.CreateGameController(createdGameMock.Object)).Returns(gameControllerMock.Object);

            _availableGames.Add(GameName, createdGameMock.Object);


            bool result = _instance.JoinGame(PlayerName, GameName);

            Assert.True(result);
            createdGameMock.Verify(cr => cr.AddPlayer(PlayerName), Times.Once);
            createdGameMock.Verify(cr => cr.IsReadyToStart(), Times.Once);
            _gameControllerFactoryMock.Verify(factory => factory.CreateGameController(It.IsAny<CreatedGame>()), Times.Once());
            Assert.True(_activeGames.ContainsKey(GameName));
            Assert.AreEqual(gameControllerMock.Object,_activeGames[GameName]);
        }

        [Test]
        public void ShouldReturnNullWhenAskingForGameStateOfNotExistingGameController()
        {


            GameState gameState = _instance.GetGameState(GameName);

            Assert.Null(gameState);

        }

        [Test]
        public void ShouldReturnGameStateOfExistingGameController()
        {
            var gameController = new Mock<IGameController>();
            var gameState = new Mock<GameState>();
            gameController.Setup(gc => gc.GameState).Returns(gameState.Object);

            _activeGames.Add(GameName, gameController.Object);

            var result = _instance.GetGameState(GameName);

            Assert.NotNull(result);
            Assert.AreEqual(result, gameState.Object);

        }

        [Test]
        public void ShouldReturnFalseWhenMakingMoveWithNullParameters()
        {
            var move = new Mock<Move>(null);

            Assert.False(_instance.MakeMove(null,GameName,move.Object));
            Assert.False(_instance.MakeMove(PlayerName,null,move.Object));
            Assert.False(_instance.MakeMove(PlayerName,GameName,null));
        }

        [Test]
        public void ShouldReturnFalseWhenMakingMoveWithEmptyStringParameters()
        {
            var move = new Mock<Move>(null);

            Assert.False(_instance.MakeMove(string.Empty, GameName, move.Object));
            Assert.False(_instance.MakeMove(PlayerName, string.Empty, move.Object));
        }

        [Test]
        public void ShouldReturnFalseWhenMakingMoveAndNoGameControllerFound()
        {
            var move = new Mock<Move>(null);

            bool result = _instance.MakeMove(PlayerName, GameName, move.Object);

            Assert.False(result);
        }

        [Test]
        public void ShouldSendMoveToMathingGameController()
        {
            var gameController = new Mock<IGameController>();
            
            var move = new Mock<Move>(null);

            gameController.Setup(gc => gc.MakeMove(PlayerName, move.Object)).Returns(true);
            _activeGames.Add(GameName, gameController.Object);

            bool result = _instance.MakeMove(PlayerName,GameName,move.Object);

            Assert.True(result);
            gameController.Verify(gc => gc.MakeMove(PlayerName,move.Object),Times.Once);

        }

    }
}
