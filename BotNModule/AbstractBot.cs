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
        protected String name;
        protected AbstractBot()
        {
            this.botLevel = BotLevel.Easy;
        }
        protected AbstractBot(BotLevel botLevel, String name)
        {
            this.botLevel = botLevel;
            this.name = name;
        }
        public abstract Move GetNextMove(GameState gameState);
        public void SendGameState(GameState gameState)
        {
            throw new NotImplementedException();
        }
        public BotLevel GetBotLevel()
        {
            return botLevel;
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}
