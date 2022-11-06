using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ball
{
    class Ball
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }
        public Ball(int x, int y, int r)
        {
            X = x;
            Y = y;
            Radius = r;
        }

    }
}
