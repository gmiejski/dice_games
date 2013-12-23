using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace ServerModule
{
    public class Server : IServer
    {

        private GameControllerFactory _gameControllerFactory;

        private readonly Dictionary<string, IGameController> _activeGames;
        private readonly Dictionary<string, CreatedGame> _availableGames;
        private readonly Dictionary<string, string> _loggedPlayers;

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
            CreatedGame createdGame = null;
            if (!_availableGames.ContainsKey(gameName))
            {
                createdGame = new CreatedGame(playerName, gameName, gameType, numberOfPlayers, numberOfBots, botLevel);

                _availableGames.Add(gameName, createdGame);
            }

            return createdGame;
        }

        public void DeleteGame(string gameName)
        {
            throw new NotImplementedException();
        }

        public bool JoinGame(string playerName, string gameName)
        {
            bool result = false;

            if (_availableGames.ContainsKey(gameName))
            {
                var createdGame = _availableGames[gameName];
                result = createdGame.AddPlayer(playerName);
                if (createdGame.IsReadyToStart())
                {
                    _availableGames.Remove(gameName);
                    var gameController = _gameControllerFactory.CreateGameController(createdGame);
                    _activeGames.Add(gameName,gameController);
                }
            }

            return result;
        }

        public void MakeMove(string playerName, string gameName, Move move)
        {
            throw new NotImplementedException();
        }

        public bool RegisterPlayer(String playerName, string contextId)
        {
            if (String.IsNullOrEmpty(contextId) || String.IsNullOrEmpty(playerName) || _loggedPlayers.ContainsKey(playerName))
            {
                return false;
            }
            else
            {
                _loggedPlayers.Add(playerName, contextId);
                return true;
            }
        }

        public void UnregisterPlayer(string playerName)
        {
            throw new NotImplementedException();
        }

        public List<CreatedGame> GetAvailableGames()
        {
            return _availableGames.Values.ToList();
        }

        public GameState GetGameState(string gameName)
        {
            throw new NotImplementedException();
        }
    }
}
