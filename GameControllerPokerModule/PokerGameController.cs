using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonInterfacesModule;

namespace GameControllerPokerModule
{
    public class PokerGameController : AbstractGameController
    {
        private Dictionary<String, Configuration> _playersDice = new Dictionary<String, Configuration>();
        private String _firstPlayer;
        private List<string> _playersOrderedList;
        private const int NumberOfTurnsInRound = 3;
        private const int NumberOfRounds = 3; 
        private int _roundsToWin;

        private int _roundIterator = 1;
        private int _turnsIterator = 1;
        private Dictionary<String, int> _playersByScoreList = new Dictionary<String, int>();

        public PokerGameController(String ownerName, String gameName, GameType gameType,
            List<String> players, List<IBot> bots, int numberOfRounds)
            : base(ownerName, gameName, gameType, players, bots)
        {
            _roundsToWin = numberOfRounds;
            foreach (var player in players)
            {
                var dice = new List<int> { 0, 0, 0, 0, 0 };
                _playersDice.Add(player, new Configuration(Hands.HighCard, 0, dice));
            }
            foreach (var bot in bots)
            {
                var dice = new List<int> { 0, 0, 0, 0, 0 };
                _playersDice.Add(bot.Name, new Configuration(Hands.HighCard, 0, dice));
            }

            _playersOrderedList = _playersDice.Keys.ToList();
            _firstPlayer = _playersOrderedList[0];

        }

        public override bool MakeMove(String playerName, Move move)
        {
            var gameState = GameState;

            if (!playerName.Equals(gameState.WhoseTurn) || move.DicesToRoll == null)
                return false;

            if (GameState.IsOver) return false;


            if ( _turnsIterator > NumberOfTurnsInRound)
            {
                _roundIterator += 1;
                _turnsIterator = 2; // already getting turn number which will match second's player move
                gameState.WinnerName = new List<string> {_firstPlayer};
                _playersByScoreList = new Dictionary<String, int> {{_firstPlayer, 0}};
            }
            else  _turnsIterator += 1;

            var nextPlayerName = GetNextPlayerName();

            var playerConfiguration = _playersDice[playerName];
            var rnd = new Random();
            foreach (var element in move.DicesToRoll)
            {
                playerConfiguration.Dices[element] = rnd.Next(1, 7); // creates a number between 1 and 6
            }

            playerConfiguration = CheckConfiguration(playerConfiguration);

            gameState.PlayerStates[playerName].Dices = playerConfiguration.Dices;

            CheckWinnerChange(playerConfiguration, playerName);
            gameState.WhoseTurn = nextPlayerName;
//            updateGameState(gameState);
            OnBroadcastGameState(GameName, GameState);

            if (_roundIterator == NumberOfRounds && nextPlayerName.Equals(_firstPlayer))
            {
                gameState.IsOver = true;
                OnDelete(GameName);
            }

            return true;

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
            PlayerState winningPlayerState = GameState.PlayerStates[GameState.WinnerName[0]];

            PlayerState playerState = GameState.PlayerStates[playerName];
//            playerState.CurrentResultValue = (int)Enum.Parse(typeof(Hands), playerState.CurrentResult) * 1000000 + playerConfiguration.HigherValue * 1000 + playerConfiguration.LowerValue;
            _playersByScoreList[playerName] = playerState.CurrentResultValue;

            Dictionary<String, int> tmpList = new Dictionary<String, int>();

            foreach (KeyValuePair<String, int> player in _playersByScoreList.OrderBy(key => key.Value))
            {
                tmpList.Add(player.Key, player.Value);
            }

            _playersByScoreList = tmpList;

            if (playerName.Equals(GameState.WinnerName))
            {
                if (!playerName.Equals(_playersByScoreList.Keys.First()))
                {
                    GameState.WinnerName = new List<string>();
                    GameState.WinnerName.Add(_playersByScoreList.Keys.First());
                    return true;
                }
                else
                    return false;
            }
            else
                if (winningPlayerState.CurrentResultValue < playerState.CurrentResultValue)
                {
                    GameState.WinnerName = new List<string>();
                    GameState.WinnerName.Add(playerName);
                    return true;
                }
                else if (winningPlayerState.CurrentResultValue == playerState.CurrentResultValue && !GameState.WinnerName.Contains(playerName))
                {
                    GameState.WinnerName.Add(playerName);
                    return true;
                }

            return false;
        }

        public void updateGameState(GameState newGameState)
        {
            GameState = newGameState;
        }

    }
}