using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonInterfacesModule
{
    public abstract class AbstractGameController : IGameController
    {

        private GameState _gameState;
        private List<IBot> _bots;
        private String _gameName;
        private GameType _gameType;
        private String _ownerName;
        private List<String> _playerNames;

        public event BroadcastGameStateHandler BroadcastGameState;
        public event DeleteGameControllerHandler DeleteGameController;

        protected virtual void OnBroadcastGameState(string gameName, GameState gameState)
        {
            if (BroadcastGameState != null)
            {
                BroadcastGameState(gameName, gameState);
            }
        }

        protected virtual void OnDelete(string gameName)
        {
            if (DeleteGameController != null)
            {
                DeleteGameController(gameName);
            }
        }

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

        public string GameName
        {
            get { return _gameName; }
        }

        protected AbstractGameController(String ownerName, String gameName, GameType gameType, List<String> players, List<IBot> bots)
        {
            if (players.Count + bots.Count == 0) {
                throw new ArgumentException();
            }
            _ownerName = ownerName;
            _gameName = gameName;
            _gameType = gameType;
            _playerNames = players;
            _bots = bots;
            GameState = new GameState();
            GameState.PlayerStates = new Dictionary<string, PlayerState>();
            foreach (IBot bot in bots)
            {
                bot.BotMoved += new BotMovedHandler(BotMoved);
            }
            GameState.IsOver = false;
            GameState.WinnerName = new List<string>() { players[0] };
            GameState.WhoseTurn = players[0];
        }


        public void BotMoved(string botName, Move move)
        {
            MakeMove(botName, move);
        }


        public abstract bool MakeMove(string playerName, Move move);
    }
}
