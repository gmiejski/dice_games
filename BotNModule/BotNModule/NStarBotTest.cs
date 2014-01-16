using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotNModule;
using CommonInterfacesModule;
using NUnit.Framework;
using Moq;

namespace BotNTestModule
{
    [TestFixture]
    class NStarBotTest
    {
        private NStarBot _botEasy;
        private NStarBot _botHard;
        private const BotLevel LevelEasy = BotLevel.Easy;
        private const BotLevel LevelHard = BotLevel.Hard;

        [SetUp]
        public void SetUp()
        {
            _botEasy = new NStarBot(LevelEasy);
            _botHard = new NStarBot(LevelHard);
        }

        [Test]
        public void TestBotLevel()
        {
            Assert.AreEqual(BotLevel.Easy, _botEasy.GetBotLevel());
            Assert.AreEqual(BotLevel.Hard, _botHard.GetBotLevel());
        }

        [Test]
        public void TestUltimateProbality()
        {
            List<int> dices = new List<int>(new int[] { 1, 1, 1, 1, 1 });
            List<int> dicesToRoll = _botHard.UltimateProbability(dices, 1, new List<int>());
            Assert.AreEqual(0, dicesToRoll.Count);

            dices = new List<int>(new int[] { 1, 1, 2, 1, 1 });
            dicesToRoll = _botHard.UltimateProbability(dices, 1, new List<int>());
            Assert.AreEqual(1, dicesToRoll.Count);
            Assert.IsTrue(dicesToRoll.Contains(2));

            dices = new List<int>(new int[] { 6, 2, 3, 4, 5 });
            dicesToRoll = _botHard.UltimateProbability(dices, 1, new List<int>());
            Assert.AreEqual(5, dicesToRoll.Count);

            dices = new List<int>(new int[] { 5, 1, 4, 2, 5 });
            dicesToRoll = _botHard.UltimateProbability(dices, 1, new List<int>());
            Assert.AreEqual(4, dicesToRoll.Count);

            dices = new List<int>(new int[] { 5, 1, 4, 2, 5 });
            dicesToRoll = _botHard.UltimateProbability(dices, 2, new List<int>());
            Assert.AreEqual(3, dicesToRoll.Count);
        }

        [Test]
        public void TestCollectSetSize()
        {
            Assert.AreEqual(1.0, _botHard.CollectSetSize(1, 4));
            Assert.AreEqual(0.0, _botHard.CollectSetSize(1, 7));
            Assert.AreEqual(0.0, _botHard.CollectSetSize(1, 0));
            Assert.AreEqual(1.0, _botHard.CollectSetSize(5, 7776));
            Assert.AreEqual(0.0, _botHard.CollectSetSize(5, 7777));
            Assert.AreEqual(1.0, _botHard.CollectSetSize(5, 1));
            Assert.AreEqual(5.0, _botHard.CollectSetSize(5, 5));
            Assert.AreEqual(25.0, _botHard.CollectSetSize(5, 6));
        }

    }
}
