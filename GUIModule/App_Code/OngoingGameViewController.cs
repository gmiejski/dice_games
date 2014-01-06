using CommonInterfacesModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUIModule;

namespace GUIModule.App_Code
{
    public class OngoingGameViewController : AbstractGameViewController
    {

        private GameState _ongoingGame;

        public OngoingGameViewController(string playerName, string gameName, GameState gameState)
            : base(playerName, gameName)
        {
            _ongoingGame = gameState;
        }

        public override GameData GetGameData()
        {
            GameData gameData = new GameData();
            gameData.Name = GameName;

            gameData.State = _ongoingGame.IsOver ? "zakończona" : "trwa";
            gameData.WhoseTurn = _ongoingGame.WhoseTurn;
            gameData.Winner = _ongoingGame.WinnerName;

            return gameData;
        }

        public override bool IsOngoing()
        {
            return true;
        }

        public override Dictionary<string, PlayerState> GetPlayers()
        {
            return _ongoingGame.PlayerStates;
        }
    }
}
