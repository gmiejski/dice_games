using System;
using System.Collections.Generic;
using System.Linq;
using CommonInterfacesModule;
using Microsoft.AspNet.SignalR;
using GUIModule;

namespace ServerModule
{
    /// <summary>
    /// Server is the central point between Hub class and actions takne on different GameControllers.
    /// Its is responsible of : <br></br>
    /// <list type="bullet">
    /// <item>delivering all actions from clients to specific game instances </item>
    /// <item>managing state of not started game</item>
    /// <item>calling Hub actions when certains events are raised from game controllers</item>
    /// </list>
    /// </summary>
    public class Server : IServer
    {

        private readonly GameControllerFactory _gameControllerFactory;

        private readonly Dictionary<string, IGameController> _activeGames;
        private readonly Dictionary<string, CreatedGame> _availableGames;
        private readonly Dictionary<string, string> _loggedPlayers;
        private readonly object _lockActiveGames = new object();
        private readonly object _lockAvailableGames = new object();
        private readonly object _lockLoggedPlayers = new object();

        /// <summary> 
        /// Class constructor used for tests.
        /// </summary>
        /// <param name="activeGames">Dictionary containing pairs of {GameName : IGameController}</param>
        /// <param name="availableGames">Dictionary containing pairs of {GameName : CreatedGame}</param>
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
            _gameControllerFactory = GameControllerFactory.getInstance();
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
        /// <param name="numberOfRounds">Rounds needed to win the game</param>
        /// <returns>Returns CreatedGame object or null when operation was unsuccesful .</returns>
        public CreatedGame CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBots, BotLevel botLevel, int numberOfRounds)
        {
            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(gameName) || numberOfPlayers < 0 || numberOfBots < 0)
            {
                return null;
            }
            CreatedGame createdGame = null;
            lock (_lockAvailableGames)
            {
                lock (_lockActiveGames)
                {
                    if (_activeGames.ContainsKey(gameName))
                    {
                        return null;
                    }
                }
                if (!_availableGames.ContainsKey(gameName))
                {
                    createdGame = new CreatedGame(playerName, gameName, gameType, numberOfPlayers, numberOfBots,
                        botLevel, numberOfRounds);

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
                    GetHubContext().Clients.Group(gameName).requestRefresh();
                    return true;
                }
            }
            lock (_lockActiveGames)
            {
                if (_activeGames.ContainsKey(gameName))
                {
                    _activeGames.Remove(gameName);
                    GetHubContext().Clients.Group(gameName).endGame();
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

            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(gameName))
            {
                return false;
            }

            bool result = false;
            lock (_lockAvailableGames)
            {
                lock (_lockLoggedPlayers)
                {
                    if (!_loggedPlayers.ContainsKey(playerName))
                    {
                        return false;
                    }

                    if (_availableGames.ContainsKey(gameName))
                    {
                        var createdGame = _availableGames[gameName];
                        result = createdGame.AddPlayer(playerName);
                        if (createdGame.IsReadyToStart())
                        {
                            _availableGames.Remove(gameName);
                            var gameController = _gameControllerFactory.CreateGameController(createdGame);
                            gameController.BroadcastGameState += OnGameStateChanged;
                            gameController.DeleteGameController += DeleteGame;
                            lock (_lockActiveGames)
                            {
                                _activeGames.Add(gameName, gameController);
                            }
                        }
                    }
                }
                if (result)
                {
                    GetHubContext().Clients.Group(gameName, _loggedPlayers[playerName]).requestRefresh();
                }
            }
            return result;
        }

        /// <summary>
        /// This method is called when human player wants to make a move.
        /// </summary>
        /// <param name="playerName">Name of player who wants to make a move</param>
        /// <param name="gameName">Name of game player belongs to</param>
        /// <param name="move">Player's move consisting of dices player wants to roll</param>
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
            if (String.IsNullOrEmpty(contextId) || String.IsNullOrEmpty(playerName))
            {
                return false;
            }
            lock (_lockLoggedPlayers)
            {
                if (_loggedPlayers.ContainsKey(playerName)) return false;

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
            if (String.IsNullOrEmpty(playerName))
            {
                return false;
            }

            lock (_lockLoggedPlayers)
            {
                if (!_loggedPlayers.ContainsKey(playerName))
                {
                    return false;
                }
                RemovePlayer(playerName);

                return _loggedPlayers.Remove(playerName);
            }
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
                foreach (var game in _availableGames.Where(game => game.Value.PlayerNames.Contains(playerName)))
                {
                    if (game.Value.PlayerNames.Count == 1)
                    {
                        DeleteGame(game.Key);
                    }
                    else
                    {
                        game.Value.PlayerNames.Remove(playerName);
                        GetHubContext().Clients.Group(game.Key).requestRefresh();
                    }
                    return true;
                }
            }
            lock (_lockActiveGames)
            {
                foreach (var game in _activeGames.Where(game => game.Value.GameState.PlayerStates.Keys.Contains(playerName)))
                {
                    DeleteGame(game.Key);
                    return true;
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
            GetHubContext().Clients.Group(gameName).requestRefresh();
        }

        private IHubContext GetHubContext()
        {
            return GlobalHost.ConnectionManager.GetHubContext<GameHub>();
        }
    }
}
