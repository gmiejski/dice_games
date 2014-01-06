using CommonInterfacesModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GUIModule.App_Code
{
    public class PendingGameViewController : AbstractGameViewController
    {

        private CreatedGame _pendingGame;

        public PendingGameViewController(string playerName, string gameName, CreatedGame createdGame)
            : base(playerName, gameName)
        {
            _pendingGame = createdGame;
        }


        public override GameData GetGameData()
        {
            GameData gameData = new GameData();
            gameData.Name = GameName;
            gameData.State = "oczekiwanie na graczy";

            return gameData;
        }

        public override bool IsOngoing()
        {
            return false;
        }

        public override Dictionary<string, PlayerState> GetPlayers()
        {
            var plrList = _pendingGame.PlayerNames;
            Dictionary<string, PlayerState> plrDict = new Dictionary<string, PlayerState>();
            foreach (var plr in plrList)
            {
                plrDict[plr] = null;
            }
            return plrDict;
        }

    }
}