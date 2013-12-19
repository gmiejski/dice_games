using System;
using System.Web;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;

namespace Gui
{
    public class GameHub : Hub
    {
        public void ThrowDice(string playerName, string gameName, int[] dice)
        {
            /* fuj */
            var random = new Random();
            var game = Global.server.GetGameState(gameName);
            var playerState = game.PlayerStates[playerName];
            var newDice = new int[5];
            var currDice = playerState.Dices.ToArray();
            for (int i = 0; i < 5; i++)
            {
                newDice[i] = (dice[i] == 1) ? (random.Next() % 6 + 1) : currDice[i];
            }

            playerState.Dices = new List<int>(newDice);
            var ord = game.PlayerStates.Last().Key == game.WhoseTurn ? 1 : 2;
            game.WhoseTurn = game.PlayerStates.ElementAt(ord).Key;
            
            // Call the broadcastMessage method to update clients.
            Clients.All.requestRefresh();
        }
    }
}
