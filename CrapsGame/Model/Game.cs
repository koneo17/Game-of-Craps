using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapsGame.Model
{
    public class Game
    {
        
        public enum statusType {WIN, LOSE, PLAY}
        public statusType Status;
        public int PlayerID;
        public int? SumOfDices;
        public int? Points;
        public Game()
        {
            Status = statusType.PLAY;
            Points = 0;
            SumOfDices = 0;
            PlayerID = 0;
        }
    }
}
