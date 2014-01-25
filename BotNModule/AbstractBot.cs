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
        public virtual event BotMovedHandler BotMoved;
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
        public abstract void SendGameState(GameState gameState);
       // {
	        //if (BotMoved != null)
            //    BotMoved(Name, GetNextMove(gameState));
       // }
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
