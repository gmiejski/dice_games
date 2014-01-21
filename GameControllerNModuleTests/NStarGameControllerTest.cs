using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameControllerNModule;
using System.Collections.Generic;
using CommonInterfacesModule;

namespace GameControllerNModuleTests
{
    [TestClass]
    public class NStarGameControllerTest
    {
        private NStarGameController controller2;
        private CommonInterfacesModule.GameType gameType2 = CommonInterfacesModule.GameType.NStar;
        private List<String> players = new List<String>() { "player1", "player2", "player3" };
        private String gameName = "game1";
        private String gameOwner = "owner";
        private List<IBot> emptyBots = new List<IBot>();

        [TestMethod]
        public void CreateGameControllerTest()
        {
            var bot1 = new Mock<IBot>();
            var bot2 = new Mock<IBot>();
            bot1.SetupGet(x => x.Name).Returns("bot1");
            bot2.SetupGet(x => x.Name).Returns("bot2");
            List<IBot> bots = new List<IBot>() { bot1.Object, bot2.Object };
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, bots);
            Assert.IsNotNull(controller2);
            Assert.IsNotNull(controller2.GameState);
            Assert.IsNotNull(controller2.GameState.PlayerStates);
            foreach (IBot bot in bots)
            {
                Assert.IsTrue(controller2.GameState.PlayerStates.ContainsKey(bot.Name));
            }
            foreach (String player in players)
            {
                Assert.IsTrue(controller2.GameState.PlayerStates.ContainsKey(player));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateGameControllerNullOwner()
        {
            new NStarGameController(null, gameName, gameType2, players, emptyBots);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateGameControllerNullName()
        {
            new NStarGameController(gameOwner, null, gameType2, players, emptyBots);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateGameControllerNullPlayers()
        {
            new NStarGameController(gameOwner, gameName, gameType2, null, emptyBots);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateGameControllerNullBots()
        {
            new NStarGameController(gameOwner, gameName, gameType2, players, null);
        }

        [TestMethod]
        public void MakeMoveWrongPlayer()
        {
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, emptyBots);
            Move move = new Move(new List<int>() { 0, 1 });
            Assert.IsFalse(controller2.MakeMove("player2", move));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MakeMoveNullPlayer()
        {
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, emptyBots);
            Move move = new Move(new List<int>());
            controller2.MakeMove(null, move);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MakeMoveWrongMoveSize()
        {
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, emptyBots);
            Move move = new Move(new List<int>() { 1, 2, 3, 4, 5, 6 });
            controller2.MakeMove("player1", move);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MakeMoveWrongMoveValues()
        {
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, emptyBots);
            Move move = new Move(new List<int>() { 7, 8 });
            controller2.MakeMove("player1", move);
        }

        [TestMethod]
        public void MakeMoveTest()
        {
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, emptyBots);
            List<int> playerState1;
            playerState1 = controller2.GameState.PlayerStates["player1"].Dices;
            Assert.IsTrue(playerState1.Contains(0));
            Move move = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            Assert.IsTrue(controller2.MakeMove("player1", move));
            playerState1 = controller2.GameState.PlayerStates["player1"].Dices;
            Assert.IsFalse(playerState1.Contains(0));
        }

        [TestMethod]
        public void BotMoveTest()
        {
            var bot1 = new Mock<IBot>();
            bot1.SetupGet(x => x.Name).Returns("bot1");
            List<IBot> bots = new List<IBot>() { bot1.Object };
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, bots);
            controller2.GameState.WhoseTurn = "bot1";
            Move move = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            bot1.Raise(m => m.BotMoved += null, "bot1", move);
            List<int> botDice = controller2.GameState.PlayerStates["bot1"].Dices;
            Assert.IsFalse(botDice.Contains(0));
        }

        [TestMethod]
        public void ChangingActivePlayerTest()
        {
            Move move = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            controller2 = new NStarGameController(gameOwner, gameName, gameType2, players, emptyBots);
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(controller2.MakeMove("player1", move));
                Assert.IsTrue(controller2.MakeMove("player2", move));
                Assert.IsTrue(controller2.MakeMove("player3", move));
            }
        }
    }
}
