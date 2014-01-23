using System.Collections.Generic;
using CommonInterfacesModule;
using BotNModule;
using BotPokerModule;

namespace ServerModule
{
    public class BotFactory
    {
        private static int index = 0;
        public static List<IBot> CreateBots(BotLevel botLevel, GameType gameType, int numberOfBots)
        {
            List<IBot> botList = new List<IBot>(numberOfBots);

            if (gameType == GameType.NPlus)
                for (int i = 0; i < numberOfBots; i++)
                    botList.Add(new NPlusBot(botLevel, "BOT#" + (index+i).ToString()));
            else if (gameType == GameType.NStar)
                for (int i = 0; i < numberOfBots; i++)
                    botList.Add(new NStarBot(botLevel, "BOT#" + (index+i).ToString()));
            else if (gameType == GameType.Poker)
              for (int i = 0; i < numberOfBots;i++)
                   botList.Add(new BotPoker(botLevel, "BOT#" + (index+i).ToString()));
            index += numberOfBots;
            return botList;
        }
        private BotFactory()
        {
        }

    }
}

