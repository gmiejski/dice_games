using System;
using System.Web;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using CommonInterfacesModule;
using System.Threading.Tasks;

namespace GUIModule
{
    public class GameHub : Hub
    {
        IServer server = Global.server;
        public void ThrowDice(string playerName, string gameName, int[] dice)
        {
            server.MakeMove(playerName, gameName, new Move(dice.ToList()));
        }

        public void LoginToGroup(string playerName, string gameName)
        {
            Groups.Add(Context.ConnectionId, gameName);
        }
    }
}
