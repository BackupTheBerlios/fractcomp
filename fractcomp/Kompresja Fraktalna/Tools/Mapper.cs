using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FractalCompression.Tools
{
    class Mapper
    {
        private double s = 0;
        private double a = 0, k = 0, d = 0, l = 0, e = 0, g = 0, h = 0, m = 0;
        private Point k1, k2, i1, i2;
        private double val1, val2, val3, val4;

        //to jest zle, nie nalezy tego jeszcze uzywac
        public Mapper(double contractivityFactor, Point k1, int val1, Point k2,
            int val2, Point i1, int val3, Point i2, int val4)
        {
            s = contractivityFactor;
            this.i1 = i1;
            this.i2 = i2;
            this.k1 = k1;
            this.k2 = k2;
            this.val1 = val1;
            this.val2 = val2;
            this.val3 = val3;
            this.val4 = val4;
            ComputeParameters();
        }

        private void ComputeParameters()
        {

        }

        public MappedPoint MapPoint(int x, int y, double val)
        {
            int mappedX = (int)(a * x + k);
            int mappedY = (int)(d * y + l);
            double mappedVal = e * x + g * y + h * x * y + s * val + m;
            return new MappedPoint(mappedX, mappedY, mappedVal);
        }

    }
}
