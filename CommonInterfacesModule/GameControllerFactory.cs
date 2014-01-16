﻿using System.Collections;



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
            

            switch (createdGame.GameType)
            {
                case GameType.NPlus:
                    return new 
                    break;

                case GameType.NStar:
                    break;

                case GameType.Poker:
                    break;

            }
        }
    }
}

