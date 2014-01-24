using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CommonInterfacesModule;

namespace GameControllerPokerModule
{
    public class PokerGameController : AbstractGameController
    {
        private Dictionary<String, Configuration> _playersDice = new Dictionary<String, Configuration>();
        private String _firstPlayer;
        private List<string> _playersOrderedList;
        private readonly int _numberOfTurnsInRound;
        private readonly int _numberOfRoundsToWin = 3; 

        private int _roundIterator = 1;
        private int _turnsIterator = 1;
        private Dictionary<String, int> _playersByScoreList = new Dictionary<String, int>();
        private List<IBot> _bots;

        private Random rnd = new Random();
        private readonly Hands _bestStartingHandAvailable = Hands.TwoPair;


        public PokerGameController(String ownerName, String gameName, GameType gameType,
            List<String> players, List<IBot> bots, int numberOfRoundsToWin)
            : base(ownerName, gameName, gameType, players, bots)
        {
            _numberOfRoundsToWin = numberOfRoundsToWin;
            foreach (var player in players)
            {
                ApplyStartingConfiguration(player);
            }
            
            foreach (var botName in bots.Select(bot => bot.Name))
            {
                ApplyStartingConfiguration(botName);
            }

            _playersOrderedList = _playersDice.Keys.ToList();
            _firstPlayer = _playersOrderedList[0];

            _bots = bots;

            _numberOfTurnsInRound = _playersOrderedList.Count() ;
        }

        

        public override bool MakeMove(String playerName, Move move)
        {
            var gameState = GameState;

            if (!playerName.Equals(gameState.WhoseTurn) || move.DicesToRoll == null)
                return false;

            if (GameState.IsOver) return false;

            if ( _turnsIterator > _numberOfTurnsInRound)
            {
                _roundIterator += 1;
                _turnsIterator = 2; // already getting turn number which will match second's player move
                gameState.LastRoundWinnerNames = new List<string> {_firstPlayer};
                _playersByScoreList = new Dictionary<String, int> {{_firstPlayer, 0}};
            }
            else  _turnsIterator += 1;

            var nextPlayerName = GetNextPlayerName();

            var playerConfiguration = ApplyNewDicesToPlayerConfiguration(playerName, move);

            playerConfiguration = CheckConfiguration(playerConfiguration);

            UpdateGameState(playerName, gameState, playerConfiguration, nextPlayerName);

            CheckWinnerChange(playerConfiguration, playerName);

            OnBroadcastGameState(GameName, GameState);

            CheckIfGameEnded(nextPlayerName, gameState);

            if (_bots.Any(bot => bot.Name.Equals(nextPlayerName)))
            {
                var nextBot = _bots.First(bot => bot.Name.Equals(nextPlayerName));

                nextBot.SendGameState(gameState);

            }

            return true;

        }

        private void UpdateGameState(string playerName, GameState gameState, Configuration playerConfiguration,
            string nextPlayerName)
        {
            gameState.PlayerStates[playerName].Dices = playerConfiguration.Dices;
            gameState.PlayerStates[playerName].CurrentResult = playerConfiguration.Hands.ToString();
            gameState.WhoseTurn = nextPlayerName;
        }

        private void CheckIfGameEnded(string nextPlayerName, GameState gameState)
        {
            if (_roundIterator == _numberOfRoundsToWin && nextPlayerName.Equals(_firstPlayer))
            {
                gameState.IsOver = true;
                OnDelete(GameName);
            }
        }

        private Configuration ApplyNewDicesToPlayerConfiguration(string playerName, Move move)
        {
            var playerConfiguration = _playersDice[playerName];
            
            foreach (var element in move.DicesToRoll)
            {
                playerConfiguration.Dices[element] = GetRandomDiceValue();
            }
            return playerConfiguration;
        }

        private string GetNextPlayerName()
        {
            return _playersOrderedList[(_turnsIterator - 1)%_playersOrderedList.Count];
        }

        public Configuration CheckConfiguration(Configuration configuration)
        {
            var counterList = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
            foreach (var element in configuration.Dices)
            {
                counterList[element] += 1;
            }
            var dices = new HashSet<int>(configuration.Dices);

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
                    if (counterList[6] == 0)
                    {
                        configuration.Hands = Hands.LowStraight;
                        configuration.HigherValue = 0;
                    }
                    else if (counterList[1] == 0)
                    {
                        configuration.Hands = Hands.HighStraight;
                        configuration.HigherValue = 0;
                    }
                    else
                    {
                        configuration.Hands = Hands.HighCard;
                        configuration.HigherValue = 6;
                    }
                    configuration.LowerValue = 0;
                    break;
            }

            return configuration;
        }


        public Boolean CheckWinnerChange(Configuration playerConfiguration, String playerName)
        {
            var winningPlayerState = GameState.PlayerStates[GameState.LastRoundWinnerNames[0]];

            var playerState = GameState.PlayerStates[playerName];
            playerState.CurrentResultValue = (int)Enum.Parse(typeof(Hands), playerState.CurrentResult) * 1000000 + playerConfiguration.HigherValue * 1000 + playerConfiguration.LowerValue;
            _playersByScoreList[playerName] = playerState.CurrentResultValue;

            var tmpDIctionary = _playersByScoreList.OrderBy(key => key.Value).ToDictionary(player => player.Key, player => player.Value);

            _playersByScoreList = tmpDIctionary;

//            GameState.CurrentlyWinningPlayerNames
            _playersByScoreList.Where(entry => entry.Value == _playersByScoreList.Max(entry2 => entry2.Value)).Select(t => t.Key);

//            if (playerName.Equals(GameState.LastRoundWinnerNames))
//            {
//                if (!playerName.Equals(_playersByScoreList.Keys.First()))
//                {
//                    GameState.LastRoundWinnerNames = new List<string> {_playersByScoreList.Keys.First()};
//                    return true;
//                }
//                return false;
//            }
//
//            if (winningPlayerState.CurrentResultValue < playerState.CurrentResultValue)
//            {
//                GameState.LastRoundWinnerNames = new List<string> {playerName};
//                return true;
//            }
//
//            if (winningPlayerState.CurrentResultValue == playerState.CurrentResultValue && !GameState.LastRoundWinnerNames.Contains(playerName))
//            {
//                GameState.LastRoundWinnerNames.Add(playerName);
//                return true;
//            }

            return false;
        }


        private Configuration GetStartingConfiguration(Hands highestHandsAvailable)
        {

            var configuration = new Configuration(Hands.HighCard, 0, new List<int>(), 0);
            do
            {
                configuration.Dices = GetRandomDicesList(5);

            } while ((int) CheckConfiguration(configuration).Hands > (int)highestHandsAvailable );

            return configuration;
        }

        private List<int> GetRandomDicesList(int dicesCount)
        {
            var result = new List<int>();
            for (int i = 0; i < dicesCount; i++)
            {
                result.Add(GetRandomDiceValue());
            }
            return result;
        }

        private int GetRandomDiceValue()
        {
            return rnd.Next(1, 7);
        }

        private void ApplyStartingConfiguration(string player)
        {
            var configuration = GetStartingConfiguration(_bestStartingHandAvailable);
            _playersDice.Add(player, configuration);
            GameState.PlayerStates[player].Dices.Clear();
            GameState.PlayerStates[player].Dices.AddRange(configuration.Dices);
            GameState.PlayerStates[player].CurrentResult = configuration.Hands.ToString();
        }

    }
}