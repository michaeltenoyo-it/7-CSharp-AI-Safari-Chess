using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariChess
{
    public class Animal
    {
        public string name;
        public int grade;
        public Image img;
        public string belongsTo;

        public Animal(string name, int n, string b, Image i)
        {
            this.name = name;
            this.grade = n;
            this.belongsTo = b;
            this.img = i;
        }

    }
}
