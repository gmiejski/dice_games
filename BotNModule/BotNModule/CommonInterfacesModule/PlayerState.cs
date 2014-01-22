using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public class PlayerState
    {
        public string CurrentResult { get; set; }

        public int CurrentResultValue { get; set; }

        public List<int> Dices { get; set; }

        public int NumberOfWonRounds { get; set; }

        public PlayerState(List<int> dices)
        {
            Dices = dices;
        }
    }
}
