using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public interface IPlayerState
    {

        string CurrentResult { get; set; }

        int CurrentRresultValue { get; set; }

        List<int> Dices { get; set; }
 
        int NumberOfWonRounds { get; set; }

    }
}
