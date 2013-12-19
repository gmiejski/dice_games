using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesModule
{
    public interface IGameState
    {
        bool IsOver { get; set; }

        Dictionary<String, IPlayerState> PlayerStates { get; set; }

        string WhoseTurn { get; set; }
        
        string WinnerName { get; set; }

        void Update(string playerName, Dictionary<int, int> dicesToRoll);
    }
}
