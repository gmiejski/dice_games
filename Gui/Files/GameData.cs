using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gui.Files
{
    public class GameData
    {
        public GameData() { }
        public GameData(string name, string state, string winner)
        {
            this.Name = name;
            this.State = state;
            this.Winner = winner;
        }

        public string Name { get; set; }
        public string State { get; set; }
        public string Winner { get; set; }
        public string WhoseTurn { get; set; }
    }
}