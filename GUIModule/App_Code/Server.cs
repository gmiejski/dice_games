using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CommonInterfacesModule;
using GUIModule.App_Code;

namespace GUIModule.App_Code
{
    public class Server : IServer
    {
        private Dictionary<string, GameState> games;
        
        public Server()
        {
            games = new Dictionary<string, GameState>();
            
            AvailableGames = new List<CreatedGame>();
        }

        public bool AddPlayer(string gameName, string playerName) { return false; }

        public CreatedGame CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBot, BotLevel botLevel)
        {
            AvailableGames.Add(new CreatedGame(playerName, gameName, gameType, numberOfPlayers, numberOfBot, botLevel));
            AvailableGames.Last().PlayerNames.Add(playerName);
            return AvailableGames.Last();
        }
        public bool DeleteGame(string gameName) {
            var ongoing = games.Remove(gameName);
            AvailableGames.RemoveAll(game => (game.GameName == gameName) &&
                (game.PlayerNames.Count == 1));

            var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            if (ongoing)
            {
                hub.Clients.Group(gameName).endGame();
            }
            else
            {
                hub.Clients.Group(gameName).requestRefresh();
            }
            return true;
        }

        public bool RemovePlayer(string playerName)
        {
            return true;
        }

        public List<CreatedGame> GetAvailableGames() { return AvailableGames; }
        public bool JoinGame(string playerName, string gameName) {
            GetAvailableGames().First().PlayerNames.Add(playerName);
            if (GetAvailableGames().First().PlayerNames.Count >= GetAvailableGames().First().NumberOfPlayers)
            {
                games.Add(gameName, new GameState { IsOver = false, WinnerName = null, PlayerStates = new Dictionary<string,PlayerState>() });
                foreach (var crName in GetAvailableGames().First().PlayerNames)
                {
                    games.Last().Value.PlayerStates.Add(crName, new PlayerState(new List<int> {0,0,0,0,0}) { CurrentResult = "poker", CurrentResultValue = 1, NumberOfWonRounds = 0 });
                }
                GetAvailableGames().Remove(GetAvailableGames().First());
                games.Last().Value.WhoseTurn = games.Last().Value.PlayerStates.Last().Key;
            }

            var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hub.Clients.Group(gameName).requestRefresh();
            return true;
        }

        public bool MakeMove(string playerName, string gameName, Move move) {
            var random = new Random();
            var game = GetGameState(gameName);
            var playerState = game.PlayerStates[playerName];

            foreach (var die in move.DicesToRoll)
            {
                playerState.Dices[die] = random.Next(6) + 1;
            }

            var ord = game.PlayerStates.Last().Key == game.WhoseTurn ? 0 : 1;
            game.WhoseTurn = game.PlayerStates.ElementAt(ord).Key;

            // Call the broadcastMessage method to update clients.
            var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hub.Clients.Group(gameName).requestRefresh();

            return true;
        }

        public bool RegisterPlayer(string playerName, string contextId) { return true; }
        public bool UnregisterPlayer(string playerName) { return false;  }

        public GameState GetGameState(string gameName)
        {
            try
            {
                return games[gameName];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<CreatedGame> AvailableGames { get; set; }
    }
}
