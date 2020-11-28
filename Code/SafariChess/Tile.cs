using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariChess
{
    public class Tile
    {
        public Animal hewan;
        public bool isLand;
        public bool isTrap;
        public bool isGoal;

        public Tile(Animal a, bool i, bool t, bool g)
        {
            this.hewan = a;
            this.isLand = i;
            this.isTrap = t;
            this.isGoal = g;
        }
    }
}
