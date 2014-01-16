using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonInterfacesModule;

namespace Dice.GameControllerPokerModule
{
    class PokerGameController : AbstractGameController
    {
        private Dictionary<String, Configuration> _playersDice = new Dictionary<String, Configuration>();

        public PokerGameController(String ownerName, String gameName, GameType gameType,
            List<String> players, List<IBot> bots)
            : base(ownerName, gameName, gameType, players, bots)
        {
            GameState = new GameState();
            foreach (String player in players)
            {
                List<int> dice = new List<int>() { 0, 0, 0, 0, 0 };
                GameState.PlayerStates.Add(player, new PlayerState(dice) { CurrentResultValue = 0, CurrentResult = Hands.HighCard.ToString(), NumberOfWonRounds = 0 });
                _playersDice.Add(player, new Configuration(Hands.HighCard, 0, dice));
            }
            GameState.IsOver = false;
            GameState.WinnerName = new List<String>() { ownerName };
            GameState.WhoseTurn = ownerName;
        }

        public override bool MakeMove(String PlayerName, Move move)
        {
            GameState gameState = GameState;

            if (!PlayerName.Equals(gameState.WhoseTurn))
                return false;
            Configuration playerConfiguration = _playersDice[PlayerName];
            Random rnd = new Random();
            foreach (int element in move.DicesToRoll)
            {
                playerConfiguration.Dices[element] = rnd.Next(1, 7); // creates a number between 1 and 6
            }

            playerConfiguration = CheckConfiguration(playerConfiguration);

            gameState.PlayerStates[PlayerName].Dices = playerConfiguration.Dices;

            CheckWinnerChange(playerConfiguration, PlayerName);
            //metoda update w GameState?
            GameState = gameState;
            return true;

        }

        private Configuration CheckConfiguration(Configuration configuration)
        {
            List<int> counterList = new List<int> { 0, 0, 0, 0, 0, 0 };
            foreach (int element in configuration.Dices)
            {
                counterList[element] += 1;
            }
            HashSet<int> dices = new HashSet<int>(configuration.Dices);

            switch (dices.Count())
            {
                case 1:
                    configuration.Hands = Hands.Five;
                    configuration.HigherValue = counterList.IndexOf(5);
                    configuration.LowerValue = 0;
                    break;
                case 2:
                    if (counterList.Contains(4))
                    {
                        configuration.Hands = Hands.Four;
                        configuration.HigherValue = counterList.IndexOf(4);
                        configuration.LowerValue = 0;
                    }
                    else
                    {
                        configuration.Hands = Hands.Full;
                        configuration.HigherValue = counterList.IndexOf(3);
                        configuration.LowerValue = counterList.IndexOf(2);
                    }
                    break;
                case 3:
                    if (counterList.Contains(3))
                    {
                        configuration.Hands = Hands.Three;
                        configuration.HigherValue = counterList.IndexOf(3);
                        configuration.LowerValue = 0;
                    }
                    else
                    {
                        configuration.Hands = Hands.TwoPair;
                        configuration.LowerValue = counterList.IndexOf(2);
                        configuration.HigherValue = counterList.IndexOf(2, configuration.LowerValue + 1);
                    }
                    break;
                case 4:
                    configuration.Hands = Hands.Pair;
                    configuration.HigherValue = counterList.IndexOf(2);
                    configuration.LowerValue = 0;
                    break;
                case 5:
                    configuration.Hands = Hands.HighCard;
                    configuration.HigherValue = counterList.IndexOf(5);
                    configuration.LowerValue = 0;
                    break;
            }

            return configuration;
        }


        private Boolean CheckWinnerChange(Configuration playerConfiguration, String playerName)
        {
            PlayerState winnerPlayerState = GameState.PlayerStates[GameState.WinnerName[0]];

            String winningConfiguration = winnerPlayerState.CurrentResult;
            Hands winningHand = (Hands)System.Enum.Parse(typeof(Hands), winningConfiguration);

            if (winningHand < playerConfiguration.Hands)
            {
                //GameState.WinnerName = playerName;
                //wyczyscic i dodac
                return true;
            }
            else
                if (winningHand.Equals(playerConfiguration.Hands))
                {
                    if (winnerPlayerState.CurrentResultValue < playerConfiguration.HigherValue)
                    {
                        //GameState.WinnerName = playerName;
                        //jw.
                        return true;
                    }
                    else return false;
                    // TODO lowerValue jak bedzie w gamestate
                }
                else
                    return false;
        }
    }
}