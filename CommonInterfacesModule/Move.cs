using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public class Move
    {
        public List<int> DicesToRoll { get; set; }

        public Move(List<int> dicesToRoll)
        {
            DicesToRoll = dicesToRoll;
        }

    }
}
