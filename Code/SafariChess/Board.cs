using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariChess
{
    public class Board : ICloneable
    {
        public Tile[,] map = new Tile[9, 7];
        public int value;

        public Board(Tile[,] map,int value)
        {
            this.map = map;
            this.value = value;
        }

        public Board()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    map[i, j] = new Tile(null, true, false, false);
                }
            }

            //pindah ke sini #Mitchell
            map[0, 2].isTrap = true;
            map[0, 3].isGoal = true;
            map[0, 4].isTrap = true;

            map[1, 3].isTrap = true;

            map[3, 1].isLand = false;
            map[3, 2].isLand = false;
            map[3, 4].isLand = false;
            map[3, 5].isLand = false;

            map[4, 1].isLand = false;
            map[4, 2].isLand = false;
            map[4, 4].isLand = false;
            map[4, 5].isLand = false;

            map[5, 1].isLand = false;
            map[5, 2].isLand = false;
            map[5, 4].isLand = false;
            map[5, 5].isLand = false;

            map[7, 3].isTrap = true;

            map[8, 2].isTrap = true;
            map[8, 3].isGoal = true;
            map[8, 4].isTrap = true;
        }

        public object Clone()
        {
            return new Board(map, value);
        }
    }
}
