using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace GameControllerNModule
{
    class NPlusGameController : AbstractGameController
    {
        private int _gameGoal;


        public NPlusGameController(string ownerName, string gameName, CommonInterfacesModule.GameType gameType, List<string> players, List<IBot> bots, int numberOfRounds) : base(ownerName, gameName, gameType, players, bots, numberOfRounds)
        {
            _gameGoal = GenerateNewGoal();
        }

        public override bool MakeMove(string playerName, Move move)
        {
            if (!_gameState.WhoseTurn.Equals(playerName))
                return false;

            var newDice = move.DicesToRoll.ToDictionary(die => die, die => DieRoll());
            _gameState.Update(playerName,newDice);

            if (CheckWinConditions(playerName))
            {
                resetDice();
            }
        }

        private int GenerateNewGoal()
        {
            var random = new Random();
            return random.Next(6, 37);
        }

        private int DieRoll()
        {
            var random = new Random();
            return random.Next(1, 7);
        }

        private bool CheckWinConditions(string playerName)
        {
            PlayerState playerState;
            _gameState.PlayerStates.TryGetValue(playerName, out playerState);

            if (playerState.CurrentResultValue.Equals(_gameGoal))
            {
                playerState.NumberOfWonRounds += 1;
            }
            else
            {
                return false;
            }

            if (playerState.NumberOfWonRounds.Equals(_gameGoal))
            {
                _gameState.WinnerName = playerName;
                _gameState.IsOver = true;
            }

            return true;

        }

        private void resetDice()
        {
            foreach (var player in _gameState.PlayerStates)
            {
                player.Value.Dices.Clear();
            }
        }




    }
}
