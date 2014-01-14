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

        /// <summary> 
        /// Class constructor used for tests.
        /// </summary>
        /// <param name="activeGames">Dictionary containing pairs of {GameName : GameController}</param>
        /// <param name="availableGames">Dictionary containing pairs of {GameName : GreatedGame}</param>
        /// <param name="gameControllerFactory">Factory of GameControllers</param>
        /// <param name="loggedPlayers">Dictionary containing pairs of {PlayerName : ContextId}</param>
        public Server(GameControllerFactory gameControllerFactory, Dictionary<string, IGameController> activeGames, Dictionary<string, CreatedGame> availableGames, Dictionary<string, string> loggedPlayers)
        {
            _gameControllerFactory = gameControllerFactory;
            _activeGames = activeGames;
            _availableGames = availableGames;
            _loggedPlayers = loggedPlayers;
        }

        /// <summary> 
        /// Class parameterless constructor. 
        /// </summary>
        public Server()
        {
            _activeGames = new Dictionary<string, IGameController>();
            _availableGames = new Dictionary<string, CreatedGame>();
            _loggedPlayers = new Dictionary<string, string>();
            _gameControllerFactory = new GameControllerFactory();
        }

        /// <summary>
        /// This method is called when a player wants to create new game.
        /// </summary>
        /// <param name="playerName">Name of player who wants to create a new game</param>
        /// <param name="gameName">Name of new game to be created</param>
        /// <param name="gameType">Type of game, can be: NPlus, NStar, Poker</param>
        /// <param name="numberOfPlayers">Number of human players including game creator</param>
        /// <param name="numberOfBots">Number of bots</param>
        /// <param name="botLevel">Bot level, can be: Easy, Hard</param>
        /// <returns>Returns CreatedGame object or null.</returns>
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

        /// <summary>
        /// This method is called when game is finished or when one of players left active game or when last player left available game
        /// </summary>
        /// <param name="gameName">Name of game to be deleted</param>
        /// <returns>Returns true indicating successful game deletion or false otherwise</returns>
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

        /// <summary>
        /// This method is called when a player wants to join available game.
        /// </summary>
        /// <param name="playerName">Name of player who wants to join game</param>
        /// <param name="gameName">Name of game player wants to join</param>
        /// <returns>Returns true indicating successful addition to game or false otherwise</returns>
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

        /// <summary>
        /// This method is called when human player wants to make a move.
        /// </summary>
        /// <param name="playerName">Name of player who wants to make a move</param>
        /// <param name="gameName">Name of game player belongs to</param>
        /// <param name="move">Players move consisting of dices player wants to roll</param>
        /// <returns>Returns true indicating successful move or false otherwise.</returns>
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

        /// <summary>
        /// This method is called when player wants to log in the system
        /// </summary>
        /// <param name="playerName">Name of player who want to log in</param>
        /// <param name="contextId">Context Id of connecting player</param>
        /// <returns>Returns true indicating successful login or false otherwise.</returns>
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

        /// <summary>
        /// This method is called when player wants to log out from the system.
        /// </summary>
        /// <param name="playerName">Name of player who wants to log out</param>
        /// <returns>Returns true indicating successful logout or false otherwise.</returns>
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

        /// <summary>
        /// This method is called when player wants to leave either available or active game.
        /// If player is the only one belonging to available game this game is deleted.
        /// if player belongs to active game this game is deleted.
        /// </summary>
        /// <param name="playerName">Name of player who wants to leave a game.</param>
        /// <returns>Returns true indicating successful player removal or false otherwise.</returns>
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

        /// <summary>
        /// This method is called when player wants to see available games.
        /// </summary>
        /// <returns>Returns list of available games.</returns>
        public List<CreatedGame> GetAvailableGames()
        {
            lock (_lockAvailableGames)
            {
                return _availableGames.Values.ToList();
            }
        }

        /// <summary>
        /// This method is called when player wants to see current game state.
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns>Returns current GameState.</returns>
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
