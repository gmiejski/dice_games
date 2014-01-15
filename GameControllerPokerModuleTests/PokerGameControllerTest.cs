using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameControllerPokerModule;
using CommonInterfacesModule;

namespace GameControllerPokerModuleTests
{
    [TestClass]
    public class PokerGameControllerTest
    {
        private IGameController gameController;

        [TestMethod]
        public void TestCreateGameController()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            var iBotMock1 = new Mock<IBot>();
            iBotMock1.SetupGet(x => x.Name).Returns("bot1");
            var iBotMock2 = new Mock<IBot>();
            iBotMock2.SetupGet(x => x.Name).Returns("bot2");
            List<IBot> botList = new List<IBot>() { iBotMock1.Object, iBotMock2.Object };
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            Assert.IsNotNull(gameController);
            Assert.IsNotNull(gameController.GameState);
            Assert.IsNotNull(gameController.GameState.PlayerStates);
            foreach (IBot bot in botList)
            {
                Assert.IsTrue(gameController.GameState.PlayerStates.ContainsKey(bot.Name));
            }
            foreach (String player in players)
            {
                Assert.IsTrue(gameController.GameState.PlayerStates.ContainsKey(player));
            }
        }

        [TestMethod]
        public void TestMakeMoveWithValidParameters()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = players[0];
            gameController.GameState.WhoseTurn = nextPlayer;
            Move firstMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            Assert.IsTrue(gameController.MakeMove(nextPlayer, firstMove));
            List<int> playerDice = gameController.GameState.PlayerStates[nextPlayer].Dices;
            gameController.GameState.WhoseTurn = nextPlayer;
            Move move = new Move(new List<int>() { 0, 3 });
            Assert.IsTrue(gameController.MakeMove(nextPlayer, move));
            List<int> playerDiceAfterMove = gameController.GameState.PlayerStates[nextPlayer].Dices;
            Assert.AreEqual(playerDice[1], playerDiceAfterMove[1]);
            Assert.AreEqual(playerDice[2], playerDiceAfterMove[2]);
            Assert.AreEqual(playerDice[4], playerDiceAfterMove[4]);
            Assert.IsTrue(playerDiceAfterMove[0] >= 1 && playerDiceAfterMove[0] <= 6);
            Assert.IsTrue(playerDiceAfterMove[3] >= 1 && playerDiceAfterMove[3] <= 6);
        }

        [TestMethod]
        public void TestMakeMoveWithLeaderChange()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = players[0];
            gameController.GameState.WhoseTurn = nextPlayer;
            Move firstMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            List<int> playerDice = gameController.GameState.PlayerStates[nextPlayer].Dices;
            Configuration bestConfiguration = new Configuration(Hands.Five, 6, new List<int>() { 6, 6, 6, 6, 6 });
            Assert.IsTrue(((PokerGameController)gameController).CheckWinnerChange(bestConfiguration, players[1]));
            Assert.IsTrue(gameController.GameState.WinnerName.Contains("player2"));
        }

        [TestMethod]
        public void TestMakeMoveNullDiceToRoll()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = players[0];
            gameController.GameState.WhoseTurn = nextPlayer;
            Move firstMove = new Move(null);
            Assert.IsFalse(gameController.MakeMove(nextPlayer, firstMove));
        }

        [TestMethod]
        public void TestMakeMoveInvalidPlayer()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = players[0];
            gameController.GameState.WhoseTurn = nextPlayer;
            Move firstMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            Assert.IsFalse(gameController.MakeMove(players[1], firstMove));
        }


        [TestMethod]
        public void TestBotMovedEvent()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            var iBotMock1 = new Mock<IBot>();
            iBotMock1.SetupGet(x => x.Name).Returns("bot1");
            List<IBot> botList = new List<IBot>() { iBotMock1.Object };
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = "bot1";
            gameController.GameState.WhoseTurn = nextPlayer;
            Move firstMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            iBotMock1.Raise(m => m.BotMoved += null, "bot1", firstMove);
            List<int> botDice = gameController.GameState.PlayerStates["bot1"].Dices;
            Assert.IsTrue(botDice.Sum() != 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateGameControllerWithoutPlayers()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>();
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController("player", gameName, gameType, players, botList);
        }

    }
}
