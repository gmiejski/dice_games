using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gui.Files;

namespace Gui.Files
{
    public interface IServer
    {
        bool AddPlayer(string gameName, string playerName);
        bool CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers);
        void DeleteGame(string gameName);
        List<CreatedGame> GetAvailableGames();
        bool JoinGame(string playerName, string gameName);
        void MakeMove(string playerName, string gameName, Move move);
        bool RegisterPlayer(string playerName, string contextId);
        void UnregisterPlayer(string playerName);
        GameState GetGameState(string gameName);
    }
}
