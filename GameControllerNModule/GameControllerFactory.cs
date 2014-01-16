using System.Collections;
using System.Collections.Generic;
using GameControllerNModule;
using GameControllerPokerModule;


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

                case GameType.NStar:
                    return new NStarGameController(createdGame.OwnerName,createdGame.GameName,createdGame.GameType,createdGame.PlayerNames,bots);

                case GameType.Poker:
                    return new PokerGameController(createdGame.OwnerName,createdGame.GameName,createdGame.GameType,createdGame.PlayerNames,bots);
                default:
                    return null;
            }
        }
    }
}

