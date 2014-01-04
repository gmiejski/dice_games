using CommonInterfacesModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gui.Files
{
    public class GameState : IGameState
    {
        public GameState()
        {
            PlayerStates = new Dictionary<string, IPlayerState>();
            //PlayerStates.Add("Jan", new PlayerState { CurrentResultValue = 1, NumberOfWonRounds = 2,   CurrentResult = "straight", Dices = new List<int> { 1, 2, 3, 4, 5 } });
            PlayerStates.Add("Edek", new PlayerState { CurrentRresultValue = 2, NumberOfWonRounds = 22,  CurrentResult = "none", Dices = new List<int> { 2, 2, 2, 4, 5 } } );
        }

        public Dictionary<string, IPlayerState> PlayerStates { get; set; }
        public bool IsOver { get; set; }
        public string WinnerName { get; set; }
        public string WhoseTurn { get; set; }

        public void Update(string playerName, Dictionary<int, int> dicesNewValues) { }
    }
}
