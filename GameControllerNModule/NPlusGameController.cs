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
        private int _gameGoal;
        private readonly Random _random = new Random();

        public NPlusGameController(string ownerName, string gameName, CommonInterfacesModule.GameType gameType, List<string> players, List<IBot> bots) : base(ownerName, gameName, gameType, players, bots)
        {
            _gameGoal = GenerateNewGoal();
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
            player.CurrentResultValue = player.Dices.Sum();

            if (CheckWinConditions(playerName))
            {
                ResetDice();
                _gameGoal = GenerateNewGoal();
            }
            OnBroadcastGameState(GameName, GameState);
            return true;
        }

        private int GenerateNewGoal()
        {
            var random = new Random();
            return random.Next(6, 37);
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
            if (!playerState.NumberOfWonRounds.Equals(_gameGoal)) return false;
            GameState.WinnerName.Add(playerName);
            GameState.IsOver = true;
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
    }
}
