namespace CommonInterfacesModule
{
    using System.Collections.Generic;
    using BotNModule;

    public class BotFactory
    {
        private static int index = 0;
        public static List<IBot> CreateBots(BotLevel botLevel, GameType gameType, int numberOfBots)
        {
            List<IBot> botList = new List<IBot>(numberOfBots);

            if (gameType == GameType.NPlus)
                for (int i = 0; i < numberOfBots; i++)
                    botList[i] = new NPlusBot(botLevel,index.ToString());
            else if (gameType == GameType.NStar)
                for (int i = 0; i < numberOfBots; i++)
                    botList[i] = new NStarBot(botLevel, index.ToString());
            // else if (gameType == GameType.Poker)
            // for (int i = 0; i < numberOfBots;i++)
            //       botList[i] = new BotPoker(botLevel);
            return botList;
        }
        private BotFactory()
        {
        }

    }
}

