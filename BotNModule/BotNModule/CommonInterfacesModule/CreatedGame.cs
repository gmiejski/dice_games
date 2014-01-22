using System;
using System.Collections.Generic;

namespace CommonInterfacesModule
{
    public class CreatedGame
    {
        public BotLevel BotLevel { get; private set; }

        public string GameName { get; private set; }

        public GameType GameType { get; private set; }

        public int NumberOfBots { get; private set; }

        public int NumberOfPlayers { get; private set; }

        public string OwnerName { get; private set; }

        public List<string> PlayerNames { get; private set; }

        public virtual bool AddPlayer(string playerName)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsReadyToStart()
        {
            throw new NotImplementedException();
        }

        public CreatedGame(string ownerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBots, BotLevel botLevel)
        {
            NumberOfPlayers = numberOfPlayers;
            NumberOfBots = numberOfBots;
            GameType = gameType;
            GameName = gameName;
            BotLevel = botLevel;
            OwnerName = ownerName;
            // TODO create Player names list
        }
    }
}
