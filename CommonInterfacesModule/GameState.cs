using System;
using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public class GameState
    {
        public bool IsOver { get; set; }

        public Dictionary<String, PlayerState> PlayerStates { get; set; }

        public string WhoseTurn { get; set; }

        public List<String> WinnerName { get; set; }

        public void Update(string playerName, Dictionary<int, int> dicesToRoll)
        {
            throw new NotImplementedException();
        }
    }
}
