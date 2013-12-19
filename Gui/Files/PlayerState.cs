using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gui.Files
{
    public class PlayerState
    {
        public int NumberOfWonRounds { get; set; }
        public int CurrentResultValue {get; set; }
        public List<int> Dices { get; set; }
        public string CurrentResult { get; set; }

        public PlayerState() {
            NumberOfWonRounds = 0;
            CurrentResult = "none";
            Dices = new List<int> {0,0,0,0,0};
            CurrentResultValue = 0;
        }
    }
}