using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameControllerNModule;
using System.Collections.Generic;
using CommonInterfacesModule;

namespace GameControllerNModuleTests
{
    [TestClass]
    public class NPlusGameControllerTest
    {
        private NPlusGameController controller;
        private List<String> players = new List<String>() { "player1", "player2", "player3" };
        private String gameName = "game1";
        private String gameOwner = "owner";
        private CommonInterfacesModule.GameType gameType = CommonInterfacesModule.GameType.NPlus;
        private List<IBot> emptyBots = new List<IBot>();

        [TestMethod]
        public void CreateGameControllerTest()
        {
            var bot1 = new Mock<IBot>();
            var bot2 = new Mock<IBot>();
            bot1.SetupGet(x => x.Name).Returns("bot1");
            bot2.SetupGet(x => x.Name).Returns("bot2");
            List<IBot> bots = new List<IBot>() { bot1.Object, bot2.Object };
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, bots);
            Assert.IsNotNull(controller);
            Assert.IsNotNull(controller.GameState);
            Assert.IsNotNull(controller.GameState.PlayerStates);
            foreach (IBot bot in bots)
            {
                Assert.IsTrue(controller.GameState.PlayerStates.ContainsKey(bot.Name));
            }
            foreach (String player in players)
            {
                Assert.IsTrue(controller.GameState.PlayerStates.ContainsKey(player));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateGameControllerNullOwner()
        {
            new NPlusGameController(null, gameName, gameType, players, emptyBots);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateGameControllerNullName()
        {
            new NPlusGameController(gameOwner, null, gameType, players, emptyBots);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateGameControllerNullPlayers()
        {
            new NPlusGameController(gameOwner, gameName, gameType, null, emptyBots);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateGameControllerNullBots()
        {
            new NPlusGameController(gameOwner, gameName, gameType, players, null);
        }

        [TestMethod]
        public void MakeMoveWrongPlayer()
        {
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, emptyBots);
            Move move = new Move(new List<int>() {0, 1});
            Assert.IsFalse(controller.MakeMove("player2", move));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MakeMoveNullPlayer()
        {
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, emptyBots);
            Move move = new Move(new List<int>());
            controller.MakeMove(null, move);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MakeMoveWrongMoveSize()
        {
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, emptyBots);
            Move move = new Move(new List<int>() {1, 2, 3, 4, 5, 6});
            controller.MakeMove("player1", move);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MakeMoveWrongMoveValues()
        {
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, emptyBots);
            Move move = new Move(new List<int>() {7, 8});
            controller.MakeMove("player1", move);
        }

        [TestMethod]
        public void MakeMoveTest()
        {
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, emptyBots);
            List<int> playerState1;
            playerState1 = controller.GameState.PlayerStates["player1"].Dices;
            Assert.IsTrue(playerState1.Contains(0));
            Move move = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            Assert.IsTrue(controller.MakeMove("player1", move));
            playerState1 = controller.GameState.PlayerStates["player1"].Dices;
            Assert.IsFalse(playerState1.Contains(0));
        }

        [TestMethod]
        public void BotMoveTest()
        {
            var bot1 = new Mock<IBot>();
            bot1.SetupGet(x => x.Name).Returns("bot1");
            List<IBot> bots = new List<IBot>() { bot1.Object };
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, bots);
            controller.GameState.WhoseTurn = "bot1";
            Move move = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            bot1.Raise(m => m.BotMoved += null, "bot1", move);
            List<int> botDice = controller.GameState.PlayerStates["bot1"].Dices;
            Assert.IsFalse(botDice.Contains(0));
        }

        [TestMethod]
        public void ChangingActivePlayerTest()
        {
            Move move = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            controller = new NPlusGameController(gameOwner, gameName, gameType, players, emptyBots);
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(controller.MakeMove("player1", move));
                Assert.IsTrue(controller.MakeMove("player2", move));
                Assert.IsTrue(controller.MakeMove("player3", move));
            }
        }
    }
}
