using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dice.GameControllerPokerModule
{
    public class Move
    {
        public List<int> dicesToRoll;
    }

    public enum GameType
    {
        NPlus, NStar, Poker
    }

    public class GameState
    {
        public bool IsOver;
        public string Winner;
        public string WhoseTurn;
        public Dictionary<string, PlayerState> PlayersStates;

        public void Update(string playerName, Dictionary<int, int> dicesNewValues);
    }

    class PlayerState
    {
        public List<int> Dices;
        public string CurrentResult;
        public int NumberOfWonRounds;
        public int CurrentResultValue;

        public PlayerState(List<int> dices, string result);
    }


    class PokerGameController
    {
        Dictionary<String, Configuration> _playersDice = new Dictionary<String, Configuration>();

        public PokerGameController(String ownerName, String gameName, GameType gameType, 
            List<iBots> bots, List<String> players)
        {

        }

        public GameState MakeMove(String PlayerName, Move move)
        {
            Configuration playerConfiguration = _playersDice[PlayerName];
            Random rnd = new Random();
            foreach (int element in move.dicesToRoll)
            {
                playerConfiguration.Dices[element] = rnd.Next(1, 7); // creates a number between 1 and 6
            }

            playerConfiguration = CheckConfiguration(playerConfiguration);
            GameState gameState = AbstractGameController.GameState;
            gameState.PlayersStates[PlayerName].Dices = playerConfiguration.Dices;

            return gameState;
        }

        private Configuration CheckConfiguration (Configuration configuration)
        {
            List<int> counterList = new List<int> {0,0,0,0,0,0};
            foreach (int element in configuration.Dices)
            {
                counterList[element] += 1;
            }
            HashSet<int> dices = new HashSet<int> (configuration.Dices);

            switch (dices.Count())
            {
                case 1:
                    configuration.Hands = Hands.Five;
                    break;
                case 2:
                    if (counterList.Contains(4))
                        configuration.Hands = Hands.Four;
                    else
                        configuration.Hands = Hands.Full;
                    break;
                case 3:
                    if (counterList.Contains(3))
                        configuration.Hands = Hands.Three;
                    else
                        configuration.Hands = Hands.TwoPair;
                    break;
                case 4:
                    configuration.Hands = Hands.Pair;
                    break;
                case 5:
                    configuration.Hands = Hands.HighCard;
                    break;
            }

            if (configuration.Hands == Hands.Five)
                configuration.HigherValue = counterList.IndexOf(5);

            if (configuration.Hands == Hands.Four)
                configuration.HigherValue = counterList.IndexOf(4);

            if (configuration.Hands == Hands.Full)
            {
                configuration.HigherValue = counterList.IndexOf(3);
                configuration.LowerValue = counterList.IndexOf(2);
            }

            if (configuration.Hands == Hands.Three)
                configuration.HigherValue = counterList.IndexOf(3);

            if (configuration.Hands == Hands.TwoPair)
            {
                configuration.LowerValue = counterList.IndexOf(2);
                configuration.HigherValue = counterList.IndexOf(2, configuration.LowerValue + 1);
            }

            if (configuration.Hands == Hands.Pair)
                configuration.HigherValue = counterList.IndexOf(2);

            return configuration;
        }
    }
}