using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FractalCompression.Tools
{
    [Serializable]
    class MappedPoint
    {
        private int x, y;
        private double originalX, originalY;

        public double OriginalY
        {
            get { return originalY; }
            set { originalY = value; }
        }

        public double OriginalX
        {
            get { return originalX; }
            set { originalX = value; }
        }
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

        public double CompareTo(Object obj)
        {
            if (obj.GetType() != this.GetType())
                throw new ArgumentException();

            MappedPoint mp = (MappedPoint)obj;
            if (this.X == mp.X && this.Y == mp.Y && this.Val == mp.Val)
                return 0;

            return this.Val.CompareTo(mp.Val);
        }
    }
}
