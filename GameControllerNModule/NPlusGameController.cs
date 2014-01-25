using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace GameControllerNModule
{
    public class NPlusGameController : AbstractGameController
    {
        private readonly int _roundsToWin;
        private readonly Random _random = new Random();
        private bool isWinner = false;

        public NPlusGameController(string ownerName, string gameName, CommonInterfacesModule.GameType gameType, List<string> players, List<IBot> bots, int numberOfRounds) : base(ownerName, gameName, gameType, players, bots)
        {
            _gameGoal = GenerateNewGoal();
            _roundsToWin = numberOfRounds;
            foreach (var player in _playerNames)
            {
                _gameState.Update(player,InitialHand(_gameGoal));
                PlayerState playerState;
                GameState.PlayerStates.TryGetValue(player, out playerState);
                var sum = playerState.Dices.Sum();
                playerState.CurrentResult = sum.ToString() + " [" + _gameGoal.ToString() + (sum - _gameGoal).ToString("+#;-#;#") + "]";
                playerState.CurrentResultValue = sum;
            }
            
        }

        public override bool MakeMove(string playerName, Move move)
        {
            if (playerName == null || move == null)
            {
                throw new ArgumentNullException();
            }
            if (move.DicesToRoll.Count > 5 || move.DicesToRoll.Max() > 4)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!GameState.WhoseTurn.Equals(playerName))
                return false;

            var newDice = move.DicesToRoll.ToDictionary(die => die, die => DieRoll());
            GameState.Update(playerName,newDice);
            PlayerState player;
            GameState.PlayerStates.TryGetValue(playerName,out player);
            var sum = player.Dices.Sum();
            player.CurrentResult = sum.ToString() + " [" + _gameGoal.ToString() + (sum - _gameGoal).ToString("+#;-#;#") + "]";
            player.CurrentResultValue = sum;

            if (CheckWinConditions(playerName))
            {
               // ResetDice();
               // _gameGoal = GenerateNewGoal();
               // isWinner=true;
                GameState.IsOver = true;
                OnDelete(GameName);
            }
            OnBroadcastGameState(GameName, GameState);
            if (!isWinner && _bots.Any(bot => bot.Name.Equals(GameState.WhoseTurn)))
            {
                var nextBot = _bots.First(bot => bot.Name.Equals(GameState.WhoseTurn));

                nextBot.SendGameState(GameState);

            }

            return true;
        }

        private int GenerateNewGoal()
        {
            var random = new Random();
            return random.Next(6, 31);
        }

        private int DieRoll()
        {
            return _random.Next(1, 7);
        }

        private bool CheckWinConditions(string playerName)
        {
            PlayerState playerState;
            GameState.PlayerStates.TryGetValue(playerName, out playerState);
            GameState.WhoseTurn = NextPlayer();

            if (playerState.CurrentResultValue.Equals(_gameGoal))
            {
                playerState.NumberOfWonRounds += 1;
            }
            else
            {
                return false;
            }
            if (playerState.NumberOfWonRounds < _roundsToWin)
            {
                GameState.LastRoundWinnerNames.Add(playerName);
                GameState.IsOver = true;
            }
            return true;

        }

        private void ResetDice()
        {
            foreach (var player in GameState.PlayerStates)
            {
                player.Value.Dices.Clear();
            }
        }

        private string NextPlayer()
        {
            var currentPlayer = GameState.WhoseTurn;
            var currentIndex = _playerNames.IndexOf(currentPlayer);
            var nextIndex = (currentIndex + 1)% _playerNames.Count;
            return _playerNames[nextIndex];
        }

        private Dictionary<int,int> InitialHand(int goal)
        {
            var hand = new Dictionary<int, int>();
            do
            {
                hand.Clear();
                for (var i = 0; i < 5; i++)
                    hand[i] = _random.Next(1, 7);
            } while (hand.Values.Sum() == goal);
            return hand;
        }
    }
}
