using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapsGame.Model
{
    public class DiceRolls
    {
        private Random rand;
        private int dice1;
        private int dice2;
        private int sumOfDices;

        public DiceRolls()
        {
            rand = new Random();
        }

        public int RollDice()
        {
            dice1 = rand.Next(1, 7);
            dice2 = rand.Next(1, 7);
            sumOfDices = dice1 + dice2;
            return sumOfDices;
        }

    }
}
