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

        private const string OwnerName = "ownerName";
        private const string GameName = "gameName";
        private GameType GameType = GameType.NPlus;
        private const int NumberOfPlayers = 3;
        private const int NumberOfBots = 1;
        private BotLevel BotLevel = BotLevel.Easy;

        [SetUp]
        public void SetUp()
        {
            _instance = new Server();
        }

        [Test]
        public void ShouldRegisterPlayer()
        {
            string playerName = "playerTestName";
            string contextId = "context";
            Assert.True(_instance.RegisterPlayer(playerName, contextId));
        }

        [Test]
        public void ShouldNotregisterPlayerWhenEmptyArgumentString()
        {
            string playerName = "playerName";
            string contextId = "context";
            Assert.False(_instance.RegisterPlayer(playerName, ""));
            Assert.False(_instance.RegisterPlayer("", contextId));
        }

        [Test]
        public void ShouldNotRegisterPlayerWhenArgumentIsNull()
        {
            string contextId = "someContext";
            string playerName = "playerName";
            Assert.False(_instance.RegisterPlayer(null, contextId));
            Assert.False(_instance.RegisterPlayer(playerName, null));
        }

        [Test]
        public void ShouldNotRegisterPlayerWhenDesiredNameIsInUse()
        {
            string playerName = "playerName";
            string contextId = "context";
            Assert.True(_instance.RegisterPlayer(playerName, contextId));
            Assert.False(_instance.RegisterPlayer(playerName, contextId));
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
        }

        [Test]
        public void ShouldCreateNewGameWhenGameNameAlreadyInUse()
        {
            CreatedGame createdGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);
            CreatedGame secondGame = _instance.CreateGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel);

            Assert.NotNull(createdGame);
            Assert.Null(secondGame);

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
