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
        private bool reverse;

        public Mapper(double contractivityFactor,
             Point pk, Point pi, int smallDelta, int bigDelta,
             int val1, int val2, int val3, int val4,
             int val5, int val6, int val7, int val8, bool reverse)
        {
            s = contractivityFactor;
            this.pk = pk;
            this.pi = pi;
            this.smallDelta = smallDelta;
            this.bigDelta = bigDelta;
            this.val1 = val1;
            this.val2 = val2;
            this.val3 = val3;
            this.val4 = val4;
            this.val5 = val5;
            this.val6 = val6;
            this.val7 = val7;
            this.val8 = val8;
            this.reverse = reverse;
            if (reverse)
                ComputeParametersForReverse();
            else
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
                   { pk.X, pk.Y, pk.X * pk.Y, 1, val5 - s * val1 },
                   { pk.X + bigDelta - 1, pk.Y, (pk.X + bigDelta - 1)*(pk.Y), 1, val6 - s * val2},
                   { pk.X, pk.Y + bigDelta - 1, pk.X*(pk.Y+bigDelta - 1), 1, val8 - s * val4},
                   { pk.X + bigDelta - 1, pk.Y + bigDelta - 1, (pk.X + bigDelta - 1)*(pk.Y + bigDelta -1), 1, val7 - s * val3}
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

        private void ComputeParametersForReverse()
        {
            a = smallDelta / (double)bigDelta;
            k = pk.X - pi.X * a;
            d = a;
            l = pk.Y - pi.Y * d;
            double[] B = new double[4];
            double[,] A = new double[,] { 
                   { pi.X, pi.Y, pi.X * pi.Y, 1, val5 - s * val1 },
                   { pi.X + bigDelta - 1, pi.Y, (pi.X + bigDelta - 1)*(pi.Y), 1, val6 - s * val2},
                   { pi.X, pi.Y + bigDelta - 1, pi.X*(pi.Y+bigDelta - 1), 1, val8 - s * val4},
                   { pi.X + bigDelta - 1, pi.Y + bigDelta - 1, (pi.X + bigDelta - 1)*(pi.Y + bigDelta -1), 1, val7 - s * val3}
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
            int mappedX;
            double orgX;
            double orgY;
            int mappedY;
            double mappedVal;
            if (!reverse)
            {
                orgX = (a * x + k);
                orgY = (d * y + l);
                mappedX = (int)(orgX);
                mappedY = (int)(orgY);

                mappedVal = (int)(e * x + g * y + h * x * y + s * val + m);
            }
            else
            {
                orgX = (x * (1 / a) - k);
                orgY = (y * (1 / d) - l);
                mappedX = (int)orgX;
                mappedY = (int)orgY;
                mappedVal = (int)(val - (e * x + g * y + h * x * y + m)) / s;
            }
          //  Console.Out.WriteLine(x + " " + y +" "+ val+"--->" + mappedX + " " + mappedY+" "+mappedVal);
            MappedPoint mapPoint = new MappedPoint(mappedX, mappedY, mappedVal);
            mapPoint.OriginalX = orgX;
            mapPoint.OriginalY = orgY;
            return mapPoint;
        }
    }
}
