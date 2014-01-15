using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonInterfacesModule
{
    public abstract class AbstractGameController : IGameController
    {

        protected GameState _gameState;
        protected List<IBot> _bots;
        protected String _gameName;
        protected GameType _gameType;
        protected String _ownerName;
        protected List<String> _playerNames;
        protected int _numberOfRounds;

        event BroadcastGameStateHandler IGameController.BroadcastGameState
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }//rozeslac GameState eventem

        event DeleteGameControllerHandler IGameController.DeleteGameController
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }//usunac gre
        //czekac na event od bota
        public GameState GameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;
            }
        }

        protected AbstractGameController(String ownerName, String gameName, GameType gameType, List<String> players, List<IBot> bots, int numberOfRounds)
        {
            _ownerName = ownerName;
            _gameName = gameName;
            _gameType = gameType;
            _playerNames = players;
            _bots = bots;
            _numberOfRounds = numberOfRounds;

        }

        public abstract bool MakeMove(string playerName, Move move);
    }
}
