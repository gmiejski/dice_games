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
            
            //CreatedGame createdGame = new CreatedGame(players[0], gameName, gameType, players.Count, botList.Count, BotLevel.Easy);
            //createdGame.PlayerNames = players;
            //gameController = GameControllerFactory.getInstance().CreateGameController(createdGame);
            
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
            List<int> bestDice = new List<int>() { 6, 6, 6, 6, 6 };
            Configuration bestConfiguration = new Configuration(Hands.Five, 6, bestDice);
            bestConfiguration.Dices = bestDice;
            Assert.AreEqual(bestConfiguration.Hands, Hands.Five);
            Assert.AreEqual(bestConfiguration.Dices, bestDice);
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

        [TestMethod]
        public void TestGameOver()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = players[0];
            gameController.GameState.WhoseTurn = nextPlayer;
            Move firstMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            for (int i = 0; i < 12; i++)
            {
                gameController.MakeMove(players[0], firstMove);
                gameController.GameState.WhoseTurn = players[0];
            }
            Assert.IsFalse(gameController.MakeMove(players[0], firstMove));
            Assert.IsTrue(gameController.GameState.IsOver);
        }

        [TestMethod]
        public void TestRoundOver()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = players[0];
            Move firstMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            for (int i = 0; i < 4; i++)
            {
                gameController.GameState.WhoseTurn = players[0];
                gameController.MakeMove(players[0], firstMove);
            }
            Assert.IsTrue(gameController.GameState.WinnerName.Contains(players[0]) && gameController.GameState.WinnerName.Count == 1);
        }

        [TestMethod]
        public void TestConfiguration()
        {
            String gameName = "gra1";
            GameType gameType = GameType.Poker;
            List<String> players = new List<String>() { "player1", "player2" };
            List<IBot> botList = new List<IBot>();
            gameController = new PokerGameController(players[0], gameName, gameType, players, botList);
            String nextPlayer = players[0];
            Move firstMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
            Configuration fiveConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 6, 6, 6, 6, 6 });
            Configuration fourConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 6, 6, 6, 6, 5 });
            Configuration fullConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 6, 6, 6, 5, 5 });
            Configuration highStraightConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 6, 5, 4, 3, 2 });
            Configuration lowStraightConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 5, 4, 3, 2, 1 });
            Configuration threeConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 6, 6, 6, 5, 4 });
            Configuration twoPairConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 6, 6, 5, 5, 4 });
            Configuration pairConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 6, 6, 5, 4, 3 });
            Configuration highCardConfiguration = new Configuration(Hands.HighCard, 0, new List<int>() { 1, 2, 3, 4, 6 });
            ((PokerGameController)gameController).CheckConfiguration(fiveConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(fourConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(fullConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(highStraightConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(lowStraightConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(threeConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(twoPairConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(pairConfiguration);
            ((PokerGameController)gameController).CheckConfiguration(highCardConfiguration);
            Assert.AreEqual(fiveConfiguration.Hands, Hands.Five);
            Assert.AreEqual(fiveConfiguration.HigherValue, 6);
            Assert.AreEqual(fiveConfiguration.LowerValue, 0);
            Assert.AreEqual(fourConfiguration.Hands, Hands.Four);
            Assert.AreEqual(fourConfiguration.HigherValue, 6);
            Assert.AreEqual(fourConfiguration.LowerValue, 0);
            Assert.AreEqual(fullConfiguration.Hands, Hands.Full);
            Assert.AreEqual(fullConfiguration.HigherValue, 6);
            Assert.AreEqual(fullConfiguration.LowerValue, 5);
            Assert.AreEqual(highStraightConfiguration.Hands, Hands.HighStraight);
            Assert.AreEqual(highStraightConfiguration.HigherValue, 0);
            Assert.AreEqual(highStraightConfiguration.LowerValue, 0);
            Assert.AreEqual(lowStraightConfiguration.Hands, Hands.LowStraight);
            Assert.AreEqual(lowStraightConfiguration.HigherValue, 0);
            Assert.AreEqual(lowStraightConfiguration.LowerValue, 0);
            Assert.AreEqual(threeConfiguration.Hands, Hands.Three);
            Assert.AreEqual(threeConfiguration.HigherValue, 6);
            Assert.AreEqual(threeConfiguration.LowerValue, 0);
            Assert.AreEqual(twoPairConfiguration.Hands, Hands.TwoPair);
            Assert.AreEqual(twoPairConfiguration.HigherValue, 6);
            Assert.AreEqual(twoPairConfiguration.LowerValue, 5);
            Assert.AreEqual(pairConfiguration.Hands, Hands.Pair);
            Assert.AreEqual(pairConfiguration.HigherValue, 6);
            Assert.AreEqual(pairConfiguration.LowerValue, 0);
            Assert.AreEqual(highCardConfiguration.Hands, Hands.HighCard);
            Assert.AreEqual(highCardConfiguration.LowerValue, 0);
            Assert.AreEqual(highCardConfiguration.HigherValue, 6);

        }

    }
}
