using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GUIModule.App_Code;
using CommonInterfacesModule;
using Moq;
using System.Collections.Generic;

namespace GUIUnitTest
{
    [TestClass]
    public class GameViewTest
    {
        [TestMethod]
        public void TestInstantiation()
        {
            var serv = new Mock<IServer>();
            serv.Setup(x => x.GetAvailableGames()).Returns(
                new List<CreatedGame> { new CreatedGame("aa", "game", GameType.NPlus, 1, 3, BotLevel.Hard) }
            );
            serv.Setup(x => x.GetGameState("game")).Returns((GameState)null);

            var inst = AbstractGameViewController.NewInstance("player", "game", serv.Object);
            Assert.IsInstanceOfType(inst, typeof(PendingGameViewController));
            Assert.IsFalse(inst.IsOngoing());

            serv = new Mock<IServer>();
            serv.Setup(x => x.GetAvailableGames()).Returns(
                new List<CreatedGame> { new CreatedGame("aa", "uhuhuhunotexist", GameType.NPlus, 1, 3, BotLevel.Hard) }
            );
            serv.Setup(x => x.GetGameState("game")).Returns(new GameState());

            var inst2 = AbstractGameViewController.NewInstance("player", "game", serv.Object);

            Assert.IsInstanceOfType(inst2, typeof(OngoingGameViewController));
            Assert.IsTrue(inst2.IsOngoing());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIncorrectInstantiation()
        {
            var serv = new Mock<IServer>();
            serv.Setup(x => x.GetAvailableGames()).Returns(
                new List<CreatedGame>()
            );
            serv.Setup(x => x.GetGameState("game")).Returns((GameState)null);

            AbstractGameViewController.NewInstance("player", "game", serv.Object);
        }

        [TestMethod]
        public void TestGameDataForOngoingGame()
        {
            var serv = new Mock<IServer>();
            serv.Setup(x => x.GetAvailableGames()).Returns(
                new List<CreatedGame>()
            );

            var dict = new Dictionary<string, PlayerState>();
            serv.Setup(x => x.GetGameState("game")).Returns(
                new GameState { WhoseTurn = "turn", WinnerName = "winner",
                    IsOver = true, PlayerStates = dict }
            );
            
            var view = AbstractGameViewController.NewInstance("player", "game", serv.Object);

            var gameData = view.GetGameData();

            Assert.IsInstanceOfType(gameData, typeof(GameData));
            Assert.AreEqual(gameData.Name, "game");
            Assert.AreEqual(gameData.WhoseTurn, "turn");
            Assert.AreEqual(gameData.Winner, "winner");
            Assert.AreEqual(gameData.State, "zakończona");

            Assert.AreEqual(view.GetPlayers(), dict);
        }

        [TestMethod]
        public void TestGameDataForPendingGame()
        {
            var gameType = GameType.NStar;
            var botLevel = BotLevel.Hard;
            var serv = new Mock<IServer>();
            
            var game = new CreatedGame("owner", "game", gameType, 2, 3, botLevel);
            game.AddPlayer("usr1");
            serv.Setup(x => x.GetAvailableGames()).Returns(
                new List<CreatedGame> { game }
            );

            serv.Setup(x => x.GetGameState("game")).Returns((GameState)null);

            var view = AbstractGameViewController.NewInstance("player", "game", serv.Object);

            var gameData = view.GetGameData();

            Assert.IsInstanceOfType(gameData, typeof(GameData));
            Assert.AreEqual(gameData.Name, "game");
            Assert.IsNull(gameData.WhoseTurn);
            Assert.IsNull(gameData.Winner);
            
            var arr = new string[2];
            view.GetPlayers().Keys.CopyTo(arr, 0);
            Assert.AreEqual(view.GetPlayers().Count, 2);
            Assert.AreEqual(arr[1], "usr1");
        }
    }
}
