using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace GameControllerNModule
{
    class NStarGameController : AbstractGameController
    {
        public NStarGameController(List<IBot> bots, string gameName, string ownerName, GameType gameType, List<string> playerNames) : base(bots, gameName, ownerName, gameType, playerNames)
        {
        }

        public override bool MakeMove(string playerName, Move move)
        {
            throw new NotImplementedException();
        }
    }
}
