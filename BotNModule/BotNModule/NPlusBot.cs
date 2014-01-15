using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace BotNModule
{
    public class NPlusBot : AbstractBot
    {
        public NPlusBot(BotLevel botLevel)
        {
            throw new NotImplementedException();
        }
        
        public override Move GetNextMove(GameState gameState)
        {
            throw new NotImplementedException();
        }

        public override event BotMovedHandler BotMoved;
    }
}
