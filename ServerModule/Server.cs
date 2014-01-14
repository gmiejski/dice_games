using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;
using Microsoft.AspNet.SignalR;

namespace ServerModule
{
    public class Server : IServer
    {

        private GameControllerFactory _gameControllerFactory;

        private readonly Dictionary<string, IGameController> _activeGames;
        private readonly Dictionary<string, CreatedGame> _availableGames;
        private readonly Dictionary<string, string> _loggedPlayers;
        private readonly object _lockActiveGames = new object();
        private readonly object _lockAvailableGames = new object();
        private readonly object _lockLoggedPlayers = new object();

        public Server(GameControllerFactory gameControllerFactory, Dictionary<string, IGameController> activeGames, Dictionary<string, CreatedGame> availableGames, Dictionary<string, string> loggedPlayers)
        {
            _gameControllerFactory = gameControllerFactory;
            _activeGames = activeGames;
            _availableGames = availableGames;
            _loggedPlayers = loggedPlayers;
        }

        public Server()
        {
            _activeGames = new Dictionary<string, IGameController>();
            _availableGames = new Dictionary<string, CreatedGame>();
            _loggedPlayers = new Dictionary<string, string>();
            _gameControllerFactory = new GameControllerFactory();
        }

        public CreatedGame CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBots, BotLevel botLevel)
        {
            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(gameName) || numberOfPlayers < 0 || numberOfBots < 0)
            {
                return null;
            }
            CreatedGame createdGame = null;
            if (!_availableGames.ContainsKey(gameName))
            {
                createdGame = new CreatedGame(playerName, gameName, gameType, numberOfPlayers, numberOfBots, botLevel);
                lock (_lockAvailableGames)
                {
                    _availableGames.Add(gameName, createdGame);
                }
            }
            return createdGame;
        }

        public bool DeleteGame(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                return false;
            }
            lock (_lockAvailableGames)
            {
                if (_availableGames.ContainsKey(gameName))
                {
                    _availableGames.Remove(gameName);
                    var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
                    hub.Clients.Group(gameName).requestRefresh();
                    return true;
                }
            }
            lock (_lockActiveGames)
            {
                if (_activeGames.ContainsKey(gameName))
                {
                    _activeGames.Remove(gameName);
                    var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
                    hub.Clients.Group(gameName).endGame();
                    return true;
                }
            }
            return false;
        }

        public bool JoinGame(string playerName, string gameName)
        {
            bool result = false;
            lock (_lockAvailableGames)
            {
                if (_availableGames.ContainsKey(gameName))
                {
                    var createdGame = _availableGames[gameName];
                    result = createdGame.AddPlayer(playerName);
                    if (createdGame.IsReadyToStart())
                    {
                        _availableGames.Remove(gameName);
                        var gameController = _gameControllerFactory.CreateGameController(createdGame);
                        gameController.BroadcastGameState += new BroadcastGameStateHandler(OnGameStateChanged);
                        gameController.DeleteGameController += new DeleteGameControllerHandler(DeleteGame);
                        lock (_lockActiveGames)
                        {
                            _activeGames.Add(gameName, gameController);
                        }
                    }
                }
            }
            if (result)
            {
                var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
                hub.Clients.Group(gameName, _loggedPlayers[playerName]).requestRefresh();
            }
            return result;
        }

        public bool MakeMove(string playerName, string gameName, Move move)
        {
            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(gameName) || move == null)
            {
                return false;
            }
            lock (_lockAvailableGames)
            {
                return _activeGames.ContainsKey(gameName) && _activeGames[gameName].MakeMove(playerName, move);
            }
        }

        public bool RegisterPlayer(String playerName, string contextId)
        {
            if (String.IsNullOrEmpty(contextId) || String.IsNullOrEmpty(playerName) || _loggedPlayers.ContainsKey(playerName))
            {
                return false;
            }
            lock (_lockLoggedPlayers)
            {
                _loggedPlayers.Add(playerName, contextId);
            }
            return true;
        }

        public bool UnregisterPlayer(string playerName)
        {
            if (String.IsNullOrEmpty(playerName) || !_loggedPlayers.ContainsKey(playerName))
            {
                return false;
            }
            RemovePlayer(playerName);
            lock (_lockLoggedPlayers)
            {
                _loggedPlayers.Remove(playerName);
            }
            return true;
        }

        public bool RemovePlayer(string playerName)
        {
            if (String.IsNullOrEmpty(playerName))
            {
                return false;
            }
            lock (_lockAvailableGames)
            {
                foreach(KeyValuePair<string, CreatedGame> game in _availableGames) 
                {
                    if (game.Value.PlayerNames.Contains(playerName))
                    {
                        if (game.Value.PlayerNames.Count == 1)
                        {
                            DeleteGame(game.Key);
                        }
                        else
                        {
                            game.Value.PlayerNames.Remove(playerName);
                            var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
                            hub.Clients.Group(game.Key).requestRefresh();
                        }
                        return true;
                    }
                }
            }
            lock (_lockActiveGames)
            {
                foreach (KeyValuePair<string, IGameController> game in _activeGames)
                {
                    if (game.Value.GameState.PlayerStates.Keys.Contains(playerName))
                    {
                        DeleteGame(game.Key);
                        return true;
                    }
                }
            }
            return false;
        }

        public List<CreatedGame> GetAvailableGames()
        {
            lock (_lockAvailableGames)
            {
                return _availableGames.Values.ToList();
            }
        }

        public GameState GetGameState(string gameName)
        {
            lock (_lockActiveGames)
            {
                return _activeGames.ContainsKey(gameName) ? _activeGames[gameName].GameState : null;
            }
        }

        private void OnGameStateChanged(string gameName)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hub.Clients.Group(gameName).endGame();
        }
    }
}
