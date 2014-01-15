using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace BotNModule
{
    public abstract class AbstractBot : IBot
    {
        private BotLevel botLevel;
        public abstract event BotMovedHandler BotMoved;
        protected AbstractBot()
        {
            throw new NotImplementedException();
        }
        protected AbstractBot(BotLevel botLevel)
        {
            throw new NotImplementedException();
        }
        public abstract Move GetNextMove(GameState gameState);
        public void SendGameState(GameState gameState)
        {
            throw new NotImplementedException();
        }
        BotLevel getBotLevel()
        {
            throw new NotImplementedException();
        }
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
