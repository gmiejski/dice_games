using System;
using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public class GameState
    {
        public bool IsOver { get; set; }

        public virtual Dictionary<String, PlayerState> PlayerStates { get; set; }

        public string WhoseTurn { get; set; }

        public List<string> LastRoundWinnerNames { get; set; }

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

        public GameState GetDeepCopy()
        {
            
            GameState state = new GameState();
            Dictionary<string, PlayerState> playerStates = new Dictionary<string, PlayerState>();
            foreach (string name in PlayerStates.Keys)
            {
                var dices = new List<int>(PlayerStates[name].Dices);

                PlayerState playerState = new PlayerState(dices);

                playerStates.Add(name, playerState);
            }

            state.PlayerStates = playerStates;

            return state;

        }
    
    }
}
