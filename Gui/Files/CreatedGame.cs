using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gui.Files
{
    public class CreatedGame
    {
        public string GameName { get; set; }
        public string OwnerName { get; set; }
        public int NumberOfBots { get; set; }
        public List<string> PlayerNames { get; set; }
        public int NumberOfPlayers { get; set; }
        public GameType GameType { get; set; }
        public BotLevel BotLevel { get; set; }

        public CreatedGame(string ownerName, string gameName, GameType gameType, int numberOfPlayers, int numberOfBots, BotLevel botLevel)  {
            GameName = gameName;
            OwnerName = ownerName;
            GameType = GameType.NStar;
            PlayerNames = new List<string> { ownerName };
            NumberOfPlayers = numberOfPlayers;
            NumberOfBots = numberOfBots;
            BotLevel = BotLevel.EASY;
        }
        public bool IsReadyToStart() { return false; }
        public Boolean AddPlayer(string playerName) { return false; }
    }
}
