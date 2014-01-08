using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dice.GameControllerPokerModule
{
    class Configuration
    {
        private Hands _hands;
        private int _higherValue;
        private int _lowerValue;
        private List<int> _dices;

        public Configuration(Hands hands, int higherValue, List<int> dices, int lowerValue = 0)
        {
            _hands = hands;
            _higherValue = higherValue;
            _lowerValue = lowerValue;
            _dices = dices;
        }

        public Hands Hands
        {
            get { return _hands; }
            set { _hands = value; }
        }

        public List<int> Dices
        {
            get { return _dices; }
            set { _dices = value; }
        }

        public int HigherValue
        {
            get { return _higherValue; }
            set { _higherValue = value; }
        }

        public int LowerValue
        {
            get { return _lowerValue; }
            set { _lowerValue = value; }
        }

    }
}
