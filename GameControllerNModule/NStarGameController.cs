using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace GameControllerNModule
{
    public class NStarGameController : AbstractGameController
    {
        private int _gameGoal;


        public NStarGameController(string ownerName, string gameName, CommonInterfacesModule.GameType gameType, List<string> players, List<IBot> bots) : base(ownerName, gameName, gameType, players, bots)
        {
            _gameGoal = GenerateNewGoal();
        }

        private int GenerateNewGoal()
        {
            var newGoal = 1;
            var random = new Random();
            for (var i = 0; i < 6; i++)
            {
                newGoal *= random.Next(1, 7);
            }
            return newGoal;
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
            GameState.Update(playerName, newDice);
            PlayerState player;
            GameState.PlayerStates.TryGetValue(playerName, out player);
            player.CurrentResultValue = player.Dices.Aggregate(1, (current, die) => current * die);

            if (CheckWinConditions(playerName))
            {
                ResetDice();
                _gameGoal = GenerateNewGoal();
            }
            OnBroadcastGameState(GameName, GameState);
            return true;
        }

        private string NextPlayer()
        {
            var currentPlayer = GameState.WhoseTurn;
            var currentIndex = _playerNames.IndexOf(currentPlayer);
            var nextIndex = (currentIndex + 1) % (_playerNames.Capacity-1);
            return _playerNames[nextIndex];
        }

        private void ResetDice()
        {
            foreach (var player in GameState.PlayerStates)
            {
                player.Value.Dices.Clear();
            }
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

        private int DieRoll()
        {
            var random = new Random();
            return random.Next(1, 7);
        }
    }
}
