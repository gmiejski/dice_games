using Gui.Pages;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CommonInterfacesModule;

namespace Gui.Files
{
    class Server : IServer
    {
        private Dictionary<string, GameState> games;
        
        public Server()
        {
            games = new Dictionary<string, GameState>();
            games.Add("Gra", new GameState { IsOver = false, WinnerName = null });

            AvailableGames = new List<ICreatedGame> { new CreatedGame("Gracz", "Nowa gra", GameType.NPlus, 1, 3, new BotLevel()) };
        }

        public bool AddPlayer(string gameName, string playerName) { return false; }

        public bool CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBot, BotLevel botLevel)
        {
            AvailableGames.Add(new CreatedGame(playerName, gameName, gameType, numberOfPlayers, 3, BotLevel.Easy));
            return true;
        }
        public void DeleteGame(string gameName) {
            games.Remove(gameName);
            AvailableGames.RemoveAll(game => game.GameName == gameName);
            var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hub.Clients.All.requestRefresh();
        }

        public List<ICreatedGame> GetAvailableGames() { return AvailableGames; }
        public bool JoinGame(string playerName, string gameName) {
            GetAvailableGames().First().PlayerNames.Add(playerName);
            if (GetAvailableGames().First().PlayerNames.Count > 2)
            {
                games.Add(gameName, new GameState { IsOver = false, WinnerName = null, PlayerStates = new Dictionary<string,IPlayerState>() });
                foreach (var crName in GetAvailableGames().First().PlayerNames)
                {
                    games.Last().Value.PlayerStates.Add(crName, new PlayerState { CurrentResult = "poker", CurrentRresultValue = 1, Dices = new List<int> {0,0,0,0,0}, NumberOfWonRounds = 0 });
                }
                GetAvailableGames().Remove(GetAvailableGames().First());
                games.Last().Value.WhoseTurn = games.Last().Value.PlayerStates.Last().Key;
            }

            var hub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hub.Clients.All.requestRefresh();
            return true;
        }
        public void MakeMove(string playerName, string gameName, IMove move) { }
        public bool RegisterPlayer(string playerName, string contextId) { return true; }
        public void UnregisterPlayer(string playerName) { }

        public IGameState GetGameState(string gameName)
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

        public List<ICreatedGame> AvailableGames { get; set; }
    }
}
