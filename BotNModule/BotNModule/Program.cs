using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotNModule
{
    class Program
    {
        static void Main(string[] args)
        {
            
            NPlusBot _botHard=new NPlusBot(CommonInterfacesModule.BotLevel.Hard,"ASD");
            List<int> dices = new List<int>(new int[] { 1,2,3,4,5 });
            List<int> dicesToRoll = _botHard.UltimateProbability(dices, 30, new List<int>());
            printList(dicesToRoll);
            Console.WriteLine(dicesToRoll.Contains(2));
            Console.ReadKey();
        }
        public static void printList(IEnumerable<int> dices)
        {
            int i = 0;
            foreach (var d in dices)
            {
                Console.WriteLine("{0} => {1}", d, i);
                i++;
            }
            Console.WriteLine();
        }
    }
}
