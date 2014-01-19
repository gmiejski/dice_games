using CommonInterfacesModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUIModule.App_Code
{
    public abstract class AbstractGameViewController
    {
        protected string PlayerName;
        protected string GameName;

        public AbstractGameViewController(string playerName, string gameName)
        {
            PlayerName = playerName;
            GameName = gameName;
        }

        public static AbstractGameViewController NewInstance(string playerName, string gameName, IServer server)
        {
            var ongoingGame = server.GetGameState(gameName);
            if (ongoingGame != null)
            {
                return new OngoingGameViewController(playerName, gameName, ongoingGame);
            }
            
            var pendingGame = (from game in server.GetAvailableGames()
                where game.GameName == gameName select game);
            if (pendingGame.Count() > 0)
            {
                return new PendingGameViewController(playerName, gameName, pendingGame.First());
            }

            throw new InvalidOperationException("Game " + gameName + " doesn't exist");
        }

        public abstract GameData GetGameData();
        public abstract bool IsOngoing();
        public abstract Dictionary<string, PlayerState> GetPlayers();

    }

}
