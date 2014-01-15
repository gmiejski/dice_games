using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonInterfacesModule;

namespace BotNModule
{
    public class NPlusBot : AbstractBot
    {
        public NPlusBot(BotLevel botLevel)
            : base(botLevel)
        {
        }

        public override Move GetNextMove(GameState gameState)
        {
            Move myMove = null;
            PlayerState player;
            if (gameState.PlayerStates.TryGetValue(this.name, out player))
                myMove = new Move(ultimateProbability(player.Dices, 20));
            else throw new NotImplementedException();//tu musi poleciec jakis wyjatek...
            
            return myMove;
        }

        public override event BotMovedHandler BotMoved;


        /// <summary>
        /// Zwraca liczebnosc zbioru takiego, ze suma oczek numOfDices kosci zwroci wartosc val.
        /// po podzieleniu tej wartosci przez Math.Pow(6, numOfDices) otrzymamy prawdopodobienstwo.
        /// </summary>
        /// <param name="numOfDices"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private double collectSetSize(int numOfDices, int val)
        {
            double s = 0;
            for (int i = 1; i <= 6; i++)
            {
                if (i <= val && numOfDices > 1)
                    s += collectSetSize(numOfDices - 1, val - i);

                else if (i == val && numOfDices == 1)
                    s = 1;
            }
            return s;
        }

        /// <summary>
        /// return indexes of dices to roll
        /// It's based on highest Probability
        /// </summary>
        /// <param name="dices"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<int> ultimateProbability(List<int> dices, int sum)
        {
            List<int> l = new List<int>(dices);
            List<double> best = new List<double>();
            while (l.Count > 0)
            {
                best.Add(collectSetSize(5 - l.Count, sum - l.Sum()) / Math.Pow(6, 5 - l.Count));

                l.Remove(l.Min());
            }
            for (int i = 0; i < 5 - best.IndexOf(best.Max()); i++)
            {
                dices[dices.IndexOf(dices.Max())] = -1;
            }
            l = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                if (dices[i] > 0)
                    l.Add(i);
            }

            return l;

        }
    }
}
