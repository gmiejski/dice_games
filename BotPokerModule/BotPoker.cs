using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfacesModule;

namespace BotPokerModule
{
    public class BotPoker : IBot
    {
        public string Name { get; set; }

        public event BotMovedHandler BotMoved;

        private BotLevel botLevel;

        private enum Hand { Bieda, Para, DwiePary, Trojka, Strit, Full, Kareta, Poker };

        public BotPoker(BotLevel botLevel, String name)
        {
            this.botLevel = botLevel;
            this.Name = name;
        }

        public void SendGameState(GameState gameState)
        {
            if (BotMoved != null)
            {
                try
                {
                    BotMoved(Name, GetNextMove(gameState));
                }
                catch (IndexOutOfRangeException e)
                {
                    BotMoved(Name, new Move(new List<int>(){0,1,2,3,4}));
                }
            }
        }

        public Move GetNextMove(GameState _gameState)
        {
            GameState gameState = _gameState.GetDeepCopy();

            PlayerState playerState;
            gameState.PlayerStates.TryGetValue(Name, out playerState);
            List<int> botDices = new List<int>(playerState.Dices);
            List<int> bestHand = FindBestHand(gameState.PlayerStates);
            Move move = new Move(new List<int>());
            
            int[] count = new int[6];
            foreach (int i in botDices)
            {
                count[i-1]++;
            }

            if (botLevel == BotLevel.Easy)
            {
                for (int i = 1; i < 5; i++)
                {
                    if (botDices.IndexOf(i) != -1 && botDices.IndexOf(i + 1) != -1 && botDices.IndexOf(i + 2) != -1)
                    {
                        move.DicesToRoll.AddRange(new List<int>(new int[] { 0, 1, 2, 3, 4 }));
                        move.DicesToRoll.Remove(botDices.IndexOf(i));
                        move.DicesToRoll.Remove(botDices.IndexOf(i + 1));
                        move.DicesToRoll.Remove(botDices.IndexOf(i + 2));

                        return move;
                    }
                }
            }
            if (InterpretDices(botDices) == Hand.Bieda && InterpretDices(bestHand) < Hand.Strit)
            {

                if (count.Max() == 1)
                {
                    if (count[1] == 0)
                    {
                        move.DicesToRoll.Add(botDices.IndexOf(1));
                        return move;
                    }
                    if (count[4] == 0)
                    {
                        move.DicesToRoll.Add(botDices.IndexOf(6));
                        return move;
                    }
                }
            }
            else if (InterpretDices(botDices) == Hand.DwiePary && InterpretDices(bestHand) < Hand.Full)
            {
                for(int i = 0; i < 6; i++)
                {
                    if (count[i] == 1)
                    {
                        move.DicesToRoll.Add(botDices.IndexOf(i + 1));
                        return move;
                    }
                }
            }
            else if (!CompareDices(botDices, bestHand) && InterpretDices(bestHand) == Hand.Poker)
            {
                int i = 0;
                while (count[i] != count.Max())
                    i++;

                if (i + 1 > bestHand[0])
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (count[botDices[j] - 1] != count.Max())
                            move.DicesToRoll.Add(j);
                    }
                    return move;
                }
                else if (count.Contains(2))
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (count[botDices[j] - 1] != 2)
                            move.DicesToRoll.Add(j);
                    }
                    return move;
                }
                else
                {
                    move.DicesToRoll.AddRange(new List<int>(new int[] { 0, 1, 2, 3, 4 }));
                    return move;
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (count[botDices[i] - 1] != count.Max())
                        move.DicesToRoll.Add(i);
                }

                return move;
            }

            return null;
        }

        public BotLevel GetBotLevel()
        {
            return this.botLevel;
        }

        private List<int> FindBestHand(Dictionary<String, PlayerState> PlayerStates)
        {
            PlayerState winner = PlayerStates.ElementAt(0).Value;
            PlayerStates.Remove(PlayerStates.ElementAt(0).Key);

            foreach (KeyValuePair<String, PlayerState> player in PlayerStates)
            {
                if (CompareDices(player.Value.Dices, winner.Dices))
                    winner = player.Value;
            }

            return winner.Dices;
        }

        private Hand InterpretDices(List<int> dices) {
            int[] count = new int[6];
            foreach (int i in dices)
            {
                count[i-1]++;
            }

            if (count.Contains(5))
            {
                return Hand.Poker;
            }
            else if (count.Contains(4))
            {
                return Hand.Kareta;
            }
            else if (count.Contains(3))
            {
                if (count.Contains(2))
                    return Hand.Full;
                else
                    return Hand.Trojka;
            }
            else if (count.Contains(2))
            {
                int tmp = 0;

                foreach (int x in count)
                {
                    if (x == 2)
                        tmp++;
                }

                if (tmp == 2)
                    return Hand.DwiePary;
                else
                    return Hand.Para;
            }
            else
            {
                bool strit = true;
                for(int x = 0; x < 2; x++)
                {
                    int i = 0;

                    while (i < 5)
                    {
                        if (count[x + i] != 1 && strit)
                            strit = false;
                        i++;
                    }

                    if (strit)
                        return Hand.Strit;
                }    
            }

            return Hand.Bieda;
        }

        //returns true if dices1 is a winning hand and false for dices2 being the winner
        public bool CompareDices(List<int> dices1, List<int> dices2)
        {
            if (InterpretDices(dices1) > InterpretDices(dices2))
            {
                return true;
            }
            else if (InterpretDices(dices1) < InterpretDices(dices2))
            {
                return false;
            }
            else
            {
                int[] count1 = new int[6];
                int[] count2 = new int[6];

                foreach (int i in dices1)
                {
                    count1[i - 1]++;
                }
                foreach (int i in dices2)
                {
                    count2[i - 1]++;
                }

                int max = count1.Max();
                int repeat;
                bool trojka;

                if (max == 3)
                    trojka = true;
                else
                    trojka = false;

                if (max == 2)
                    repeat = 1;
                else if (max == 1)
                    repeat = 4;
                else
                    repeat = 0;

                while (max > 0)
                {
                    int index1 = 5, index2 = 5;

                    while (index1 >= 0 && index2 >= 0)
                    {
                        if (count1[index1] == max)
                        {
                            if (count2[index2] != max)
                                return true;
                            break;
                        }
                        else if (count2[index2] == max)
                        {
                            return false;
                        }

                        index1--;
                        index2--;
                    }

                    if (repeat > 0)
                        repeat--;
                    else
                        if (max == 1 && trojka)
                            trojka = false;
                        else
                            max--;
                }

                return true;
            }
        }
    }
}
