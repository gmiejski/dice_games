using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;
using NUnit.Framework;

namespace ServerTestModule
{
    [TestFixture]
    public class CreatedGameTest
    {

        private CreatedGame _instance;
        private const string OwnerName = "ownerName";
        private const string GameName = "gameName";
        private GameType GameType = GameType.NPlus;
        private const int NumberOfPlayers = 3;
        private const int NumberOfBots = 1;
        private BotLevel BotLevel = BotLevel.Easy;
        private const int NumberOfRounds = 3;

        [SetUp]
        public void SetUp()
        {
            _instance = GetDefaultGame();
        }

        [Test]
        public void ShouldAddGameOwnerToPlayerListDuringConstruction()
        {
            Assert.True(_instance.PlayerNames.Contains(OwnerName));
        }

        [Test]
        public void ShouldAddNewPlayerToCreatedGame()
        {
            string newPlayerName = "new player";
            bool result = _instance.AddPlayer(newPlayerName);
            Assert.True(result);
            Assert.True(_instance.PlayerNames.Contains(newPlayerName));
        }

        [Test]
        public void ShouldNotAddSamePlayerMoreThanOnce()
        {
            string newPlayerName = "new player";

            Assert.True(_instance.AddPlayer(newPlayerName));
            Assert.False(_instance.AddPlayer(newPlayerName));

            Assert.AreEqual(1, _instance.PlayerNames.Count(s => s.Equals(newPlayerName)));
        }

        [Test]
        public void ShouldBeReadyToStartWhenProperNumberOfPlayers()
        {
            _instance.AddPlayer("player 2");
            _instance.AddPlayer("player 3");
            bool isReadyToStart = _instance.IsReadyToStart();

            Assert.True(isReadyToStart);
        }

        [Test]
        public void ShouldNotBeReadyToStartWhenNotFullyFilledWithPlayers()
        {
            _instance.AddPlayer("player 2");
            bool isReadyToStart = _instance.IsReadyToStart();

            Assert.False(isReadyToStart);
        }

        [Test]
        public void ShouldNotAddNewPlayerWhenGameIsReadyToStart()
        {
            _instance.AddPlayer("player 1");
            _instance.AddPlayer("player 2");
    
            Assert.True(_instance.IsReadyToStart());
            Assert.False(_instance.AddPlayer("player 3"));
        }

        private CreatedGame GetDefaultGame()
        {
            return new CreatedGame(OwnerName, GameName, GameType, NumberOfPlayers, NumberOfBots, BotLevel,NumberOfRounds);
        }

    }
}
