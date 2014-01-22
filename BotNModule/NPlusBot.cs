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
        public NPlusBot(BotLevel botLevel, String name)
            : base(botLevel,name)
        {
        }

        public override Move GetNextMove(GameState gameState)
        {
            Move myMove = null;
            PlayerState player;
            if (GetBotLevel() == BotLevel.Hard)
            {
                if (gameState.PlayerStates.TryGetValue(this.name, out player))
                {
                    List<int> l = UltimateProbability(player.Dices, 20, new List<int>());
                    List<int> ll = new List<int>(l);
                    List<int> tmpl = new List<int>(l);
                    List<int> tmpDice = new List<int>(player.Dices);
                    foreach (int x in l)
                    {
                        ll[tmpl.IndexOf(x)] = tmpDice.IndexOf(x);
                        tmpl[tmpl.IndexOf(x)] = -1;
                        tmpDice[tmpDice.IndexOf(x)] = -1;
                    }
                    myMove = new Move(ll);

                    best = new List<double>();
                    removing = new List<List<int>>();
                }
                else throw new NotImplementedException();//tu musi poleciec jakis wyjatek...
            }
            else
            {
                if (gameState.PlayerStates.TryGetValue(this.name, out player))
                    myMove = new Move(new List<int>() { 0, 1, 2, 3, 4 });
                else throw new NotImplementedException();//tu musi poleciec jakis wyjatek...

            }

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
        public double CollectSetSize(int numOfDices, int val)
        {
            double s = 0;
            for (int i = 1; i <= 6; i++)
            {
                if (i <= val && numOfDices > 1)
                    s += CollectSetSize(numOfDices - 1, val - i);

                else if (i == val && numOfDices == 1)
                    s = 1;
            }
            return s;
        }

        private List<double> best = new List<double>();
        private List<List<int>> removing = new List<List<int>>();
        /// <summary>
        /// return values of dices to roll
        /// It's based on highest Probability
        /// </summary>
        /// <param name="dices"></param>
        /// <param name="product"></param>
        /// <param name="removed">empty list</param>
        /// <returns></returns>
        public List<int> UltimateProbability(List<int> dices, int product, List<int> removed)
        {
            foreach (int x in dices)
            {
                List<int> l = new List<int>(dices);
                l.Remove(x);
                removed.Add(x);
                UltimateProbability(l, product, removed);
                best.Add(CollectSetSize(5 - l.Count, product - l.Sum()) / Math.Pow(6, 5 - l.Count));
                removing.Add(new List<int>(removed));
                removed.Remove(x);
            }

            if (best.Count > 0 && removing.Count > 0)
                return removing.ElementAt(best.IndexOf(best.Max()));
            return null;
        }
    }
}
