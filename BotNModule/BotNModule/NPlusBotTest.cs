using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotNModule;
using Moq;
using CommonInterfacesModule;
using NUnit.Framework;

namespace BotNTestModule
{
    [TestFixture]
    class BotNTest
    {

        private NPlusBot _botEasy;
        private NPlusBot _botHard;
        private const BotLevel LevelEasy = BotLevel.Easy;
        private const BotLevel LevelHard = BotLevel.Hard;

        [SetUp]
        public void SetUp()
        {
            _botEasy = new NPlusBot(LevelEasy,"1");
            _botHard = new NPlusBot(LevelHard,"2");
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
            List<int> dices = new List<int>(new int[] { 6, 6, 6, 6, 6 });
            List<int> dicesToRoll = _botHard.UltimateProbability(dices, 30, new List<int>());
            Assert.AreEqual(1, dicesToRoll.Count);

            dices = new List<int>(new int[] { 6, 2, 6, 6, 6 });
            dicesToRoll = _botHard.UltimateProbability(dices, 30, new List<int>());
            Assert.AreEqual(1, dicesToRoll.Count);
            Assert.IsTrue(dicesToRoll.Contains(2));

            dices = new List<int>(new int[] { 1, 2, 3, 4, 5 });
            dicesToRoll = _botHard.UltimateProbability(dices, 30, new List<int>());
            Assert.AreEqual(5, dicesToRoll.Count);

            dices = new List<int>(new int[] { 5, 1, 4, 2, 5 });
            dicesToRoll = _botHard.UltimateProbability(dices, 5, new List<int>());
            Assert.AreEqual(4, dicesToRoll.Count);

            dices = new List<int>(new int[] { 5, 1, 4, 2, 5 });
            dicesToRoll = _botHard.UltimateProbability(dices, 6, new List<int>());
            Assert.AreEqual(3, dicesToRoll.Count);
        }

        [Test]
        public void TestCollectSetSize()
        {
            Assert.AreEqual(1.0, _botHard.CollectSetSize(1, 4));
            Assert.AreEqual(0.0, _botHard.CollectSetSize(1, 7));
            Assert.AreEqual(0.0, _botHard.CollectSetSize(1, 0));
            Assert.AreEqual(1.0, _botHard.CollectSetSize(5, 30));
            Assert.AreEqual(0.0, _botHard.CollectSetSize(5, 31));
            Assert.AreEqual(0.0, _botHard.CollectSetSize(5, 4));
            Assert.AreEqual(6.0, _botHard.CollectSetSize(2, 7));
            Assert.AreEqual(5.0, _botHard.CollectSetSize(2, 6));
        }
    }
}
