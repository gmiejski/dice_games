using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace BotNModule
{
    class NStarBot : AbstractBot
    {
        public NStarBot(BotLevel botLevel)
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
