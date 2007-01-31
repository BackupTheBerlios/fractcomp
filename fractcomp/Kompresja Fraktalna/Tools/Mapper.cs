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
        private Point pk, pi;
        private int smallDelta, bigDelta;
        private double val1, val2, val3, val4, val5, val6, val7, val8;

        public Mapper(double contractivityFactor,
             Point pk, Point pi, int smallDelta, int bigDelta,
             int val1, int val2, int val3, int val4,
             int val5, int val6, int val7, int val8)
        {
            s = contractivityFactor;
            this.pk = pk;
            this.pi = pi;
            this.smallDelta = smallDelta;
            this.bigDelta = bigDelta;
            this.val1 = val1 / (double)255;
            this.val2 = val2 / (double)255;
            this.val3 = val3 / (double)255;
            this.val4 = val4 / (double)255;
            this.val5 = val5 / (double)255;
            this.val6 = val6 / (double)255;
            this.val7 = val7 / (double)255;
            this.val8 = val8 / (double)255;
            ComputeParameters();
        }

        private void ComputeParameters()
        {
            a = smallDelta / (double)bigDelta;
            k = pi.X - pk.X * a;
            d = a;
            l = pi.Y - pk.Y * d;
            double[] B = new double[4];
            double[,] A = new double[,] { 
                   { val1, pk.Y, pk.X * pk.Y, 1, val5 - s * val1 },
                   { val2, pk.Y, (pk.X+bigDelta)*(pk.Y), 1, val6 - s * val2},
                   { val3, pk.Y+bigDelta, pk.X*(pk.Y+bigDelta), 1, val8 - s * val3},
                   { val4, pk.Y+bigDelta, (pk.X+bigDelta)*(pk.Y+bigDelta), 1, val7 - s * val4}
            };
            if (Gauss.GaussianElimination(A, B))
            {
                e = B[0];
                g = B[1];
                h = B[2];
                m = B[3];
            }
            else
               throw new Exception("Macierz osobliwa");

        }

        public MappedPoint MapPoint(int x, int y, double val)
        {
            double temp = val / 255;
            int mappedX = (int)(a * x + k);
            int mappedY = (int)(d * y + l);
            double mappedVal = (e * x + g * y + h * x * y + s * temp + m);
            mappedVal = mappedVal * 255;
            return new MappedPoint(mappedX, mappedY, mappedVal);
        }
    }
}
