using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public interface IServer
    {
        CreatedGame CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBots, BotLevel botLevel);

        void DeleteGame(string gameName);

        bool JoinGame(string playerName, string gameName);

        void MakeMove(string playerName, string gameName, Move move);

        bool RegisterPlayer(string playerName, string contextId);

        void UnregisterPlayer(string playerName);

        List<CreatedGame> GetAvailableGames();

        GameState GetGameState(string gameName);
    }
}
