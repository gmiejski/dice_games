namespace CommonInterfacesModule
{
    using System.Collections.Generic;

    public class BotFactory
    {
        public static List<IBot> CreateBots(BotLevel botLevel, GameType gameType, int numberOfBots)
        {
            throw new System.NotImplementedException();
        }

        private BotFactory()
        {
        }

    }
}

