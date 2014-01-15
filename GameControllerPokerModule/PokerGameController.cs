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
        private int _roundIterator = 0;
        private int _turnsIterator = 0;
        private Dictionary<String, int> _playersByScoreList = new Dictionary<String, int>();

        public PokerGameController(String ownerName, String gameName, GameType gameType,
            List<String> players, List<IBot> bots)
            : base(ownerName, gameName, gameType, players, bots)
        {
            _firstPlayer = players[0];
            foreach (String player in players)
            {
                List<int> dice = new List<int>() { 0, 0, 0, 0, 0 };
                GameState.PlayerStates.Add(player, new PlayerState(dice) { CurrentResultValue = 0, CurrentResult = Hands.HighCard.ToString(), NumberOfWonRounds = 0 });
                _playersDice.Add(player, new Configuration(Hands.HighCard, 0, dice));
            }
            foreach (IBot bot in bots)
            {
                List<int> dice = new List<int>() { 0, 0, 0, 0, 0 };
                GameState.PlayerStates.Add(bot.Name, new PlayerState(dice) { CurrentResultValue = 0, CurrentResult = Hands.HighCard.ToString(), NumberOfWonRounds = 0 });
                _playersDice.Add(bot.Name, new Configuration(Hands.HighCard, 0, dice));
            }
        }

        public override bool MakeMove(String PlayerName, Move move)
        {
            GameState gameState = GameState;

            if (!PlayerName.Equals(gameState.WhoseTurn) || move.DicesToRoll == null)
                return false;

            if (_roundIterator == 4)
            {
                gameState.IsOver = true;
                OnDelete(GameName);
            }
            else
                if (PlayerName.Equals(_firstPlayer) && _turnsIterator == 4)
                {
                    _roundIterator += 1;
                    _turnsIterator = 1;
                    gameState.WinnerName = new List<string>();
                    gameState.WinnerName.Add(_firstPlayer);
                    _playersByScoreList = new Dictionary<String, int>();
                    _playersByScoreList.Add(_firstPlayer, 0);
                    return false;
                }
                else
                    if (PlayerName.Equals(_firstPlayer))
                        _turnsIterator += 1;

            Configuration playerConfiguration = _playersDice[PlayerName];
            Random rnd = new Random();
            foreach (int element in move.DicesToRoll)
            {
                playerConfiguration.Dices[element] = rnd.Next(1, 7); // creates a number between 1 and 6
            }

            playerConfiguration = CheckConfiguration(playerConfiguration);

            gameState.PlayerStates[PlayerName].Dices = playerConfiguration.Dices;

            CheckWinnerChange(playerConfiguration, PlayerName);

            updateGameState(gameState);
            OnBroadcastGameState(GameName, GameState);
            return true;

        }

        public Configuration CheckConfiguration(Configuration configuration)
        {
            List<int> counterList = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
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


        public Boolean CheckWinnerChange(Configuration playerConfiguration, String playerName)
        {
            PlayerState winningPlayerState = GameState.PlayerStates[GameState.WinnerName[0]];

            PlayerState playerState = GameState.PlayerStates[playerName];
            playerState.CurrentResultValue = (int)Enum.Parse(typeof(Hands), playerState.CurrentResult) * 1000000 + playerConfiguration.HigherValue * 1000 + playerConfiguration.LowerValue;
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
                else if (winningPlayerState.CurrentResultValue == playerState.CurrentResultValue)
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