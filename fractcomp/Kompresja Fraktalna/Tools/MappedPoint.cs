using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FractalCompression.Tools
{
    class MappedPoint
    {
        private int x, y;
        private double val;

        public double Val
        {
            get { return val; }
        }

        public int Y
        {
            get { return y; }
        }

        public int X
        {
            get { return x; }
        }

        static public implicit operator Point(MappedPoint mp)
        {
            return new Point(mp.X, mp.Y);
        }


        public MappedPoint(int x, int y, double val)
        {
            this.x = x;
            this.y = y;
            this.val = val;
        }
    }
}
