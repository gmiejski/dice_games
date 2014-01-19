using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public interface IServer
    {
        CreatedGame CreateGame(string playerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBots, BotLevel botLevel);

        bool DeleteGame(string gameName);

        bool JoinGame(string playerName, string gameName);

        bool MakeMove(string playerName, string gameName, Move move);

        bool RegisterPlayer(string playerName, string contextId);

        bool UnregisterPlayer(string playerName);

        bool RemovePlayer(string playerName);

        List<CreatedGame> GetAvailableGames();

        GameState GetGameState(string gameName);
    }
}
