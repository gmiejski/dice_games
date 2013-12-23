using System.Linq;
using CommonInterfacesModule;
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
        private GameControllerFactory _gameControllerFactory;

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
            _gameControllerFactory = new GameControllerFactory();

            _instance = new Server(_gameControllerFactory, _activeGames, _availableGames, _loggedPlayers);
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



    }
}
