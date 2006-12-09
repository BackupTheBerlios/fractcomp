using System;
using System.Collections.Generic;
using System.Text;

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

        

        public MappedPoint(int x, int y, double val)
        {
            this.x = x;
            this.y = y;
            this.val = val;
        }
    }
}
