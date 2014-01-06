using System;
using System.Web;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using CommonInterfacesModule;

namespace GUIModule
{
    public class GameHub : Hub
    {
        public void ThrowDice(string playerName, string gameName, int[] dice)
        {
            var server = Global.server;

            server.MakeMove(playerName, gameName, new Move(dice.ToList()));
        }
    }
}
