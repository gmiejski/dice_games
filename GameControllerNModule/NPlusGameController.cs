using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace GameControllerNModule
{
    class NPlusGameController : AbstractGameController
    {
        public NPlusGameController(string ownerName, string gameName, CommonInterfacesModule.GameType gameType, List<string> players, List<IBot> bots) : base(ownerName, gameName, gameType, players, bots)
        {
        }

        public override bool MakeMove(string playerName, Move move)
        {
            throw new NotImplementedException();
        }
    }
}
