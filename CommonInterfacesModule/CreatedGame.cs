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

        public int NumberOfRounds { get; private set; }

        public string OwnerName { get; private set; }

        public virtual List<string> PlayerNames { get; private set; }

        public virtual bool AddPlayer(string playerName)
        {
            if (PlayerNames.Contains(playerName) || IsReadyToStart())
            {
                return false;
            }
            PlayerNames.Add(playerName);
            return true;
        }

        public virtual bool IsReadyToStart()
        {
            if (NumberOfPlayers == PlayerNames.Count)
            {
                return true;
            }
            return false;
        }

        public CreatedGame(string ownerName, string gameName, GameType gameType,
            int numberOfPlayers, int numberOfBots, BotLevel botLevel, int numberOfRounds)
        {
            NumberOfPlayers = numberOfPlayers;
            NumberOfBots = numberOfBots;
            GameType = gameType;
            GameName = gameName;
            BotLevel = botLevel;
            NumberOfRounds = numberOfRounds;
            OwnerName = ownerName;
            PlayerNames = new List<string> { ownerName };
     
        }
    }
}
