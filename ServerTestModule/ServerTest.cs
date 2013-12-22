using CommonInterfacesModule;
using NUnit.Framework;
using ServerModule;

namespace ServerTestModule
{
    [TestFixture]
    public class ServerTest
    {

        private IServer _instance;

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
    }
}
