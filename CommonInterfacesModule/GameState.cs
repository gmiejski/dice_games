using System;
using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public class GameState
    {
        public bool IsOver { get; set; }

        public Dictionary<String, PlayerState> PlayerStates { get; set; }
        
        public string WhoseTurn { get; set; }

        public List<string> WinnerName { get; set; }

        public void Update(string playerName, Dictionary<int, int> newDice)
        {
            PlayerState player;
            if (!PlayerStates.TryGetValue(playerName, out player))
            {
                throw new ArgumentException("No such player!");
            }
            foreach (var newDie in newDice)
            {
                player.Dices[newDie.Key] = newDie.Value;
            }
        }
    }
}
