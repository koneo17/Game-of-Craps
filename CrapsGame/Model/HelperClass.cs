using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapsGame.Model
{
    static class HelperClass
    {
        private static int? _currentPlayerID;

        public static int? CurrentPlayerID
        {
            get { return _currentPlayerID; }
            set { _currentPlayerID = value; }
        }
    }
}
