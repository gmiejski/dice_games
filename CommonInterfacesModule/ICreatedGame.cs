using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesModule
{
    public interface ICreatedGame
    {
        BotLevel BotLevel { get; }

        string GameName { get; }

        GameType GameType { get; }

        int NumberOfBots { get; }

        int NumberOfPlayers { get; }

        string OwnerName { get;}

        List<string> PlayerNames { get; }

        bool AddPlayer(string playerName);

        bool IsReadyToStart();

    }
}
