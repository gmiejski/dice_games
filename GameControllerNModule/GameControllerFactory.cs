using System.Collections;
using System.Collections.Generic;
using GameControllerNModule;


namespace CommonInterfacesModule
{
    public class GameControllerFactory
    {
        static public GameControllerFactory instance = null;
        public static GameControllerFactory getInstance()
        {
            if(instance==null)
                instance = new GameControllerFactory();
            return instance;
        }
        private GameControllerFactory()
        {
            
        }
        public virtual IGameController CreateGameController(CreatedGame createdGame)
        {
            
            List<IBot> bots = BotFactory.CreateBots(createdGame.BotLevel, createdGame.GameType, createdGame.NumberOfBots);
            switch (createdGame.GameType)
            {
                case GameType.NPlus:
                    return new NPlusGameController(createdGame.OwnerName,createdGame.GameName,createdGame.GameType,createdGame.PlayerNames,bots);
                    break;

                case GameType.NStar:
                    return new NStarGameController(createdGame.OwnerName,createdGame.GameName,createdGame.GameType,createdGame.PlayerNames,bots);
                    break;

                case GameType.Poker:
                    return null;
                    break;

            }
        }
    }
}

