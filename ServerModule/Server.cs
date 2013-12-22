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

        private readonly Dictionary<string, IGameController> _activeGames;
        private readonly Dictionary<string, CreatedGame> _availableGames;
        private readonly Dictionary<string, string> _loggedPlayers;


        public Server()
        {
            _activeGames = new Dictionary<string, IGameController>();
            _availableGames = new Dictionary<string, CreatedGame>();
            _loggedPlayers = new Dictionary<string, string>();
        }

        public bool CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBots, BotLevel botLevel)
        {
            throw new NotImplementedException();
        }

        public void DeleteGame(string gameName)
        {
            throw new NotImplementedException();
        }

        public bool JoinGame(string playerName, string gameName)
        {
            throw new NotImplementedException();
        }

        public void MakeMove(string playerName, string gameName, Move move)
        {
            throw new NotImplementedException();
        }

        public bool RegisterPlayer(String playerName, string contextId)
        {
            if (String.IsNullOrEmpty(contextId) || String.IsNullOrEmpty(playerName))
            {
                return false;
            }

            if (_loggedPlayers.ContainsKey(playerName))
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
            throw new NotImplementedException();
        }

        public GameState GetGameState(string gameName)
        {
            throw new NotImplementedException();
        }
    }
}
