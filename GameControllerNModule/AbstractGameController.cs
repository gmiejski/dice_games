using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace GameControllerNModule
{
    abstract class AbstractGameController
    {
        protected List<IBot> _bots;
        protected String _gameName;
        protected GameType _gameType;
        protected String _ownerName;
        protected List<String> _playerNames;

        protected AbstractGameController(List<IBot> bots, string gameName, string ownerName, GameType gameType, List<string> playerNames)
        {
            _bots = bots;
            _gameName = gameName;
            _ownerName = ownerName;
            _gameType = gameType;
            _playerNames = playerNames;
        }
    }
}
