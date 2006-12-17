using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using FractalCompression.Structure;

namespace FractalCompression.Tools
{
    class POTools
    {
        public static bool CheckConditionOfContinuity(Domain[,] domains, double[,] s, int a, Bitmap bitmap)
        {
            for (int i = 0; i < domains.GetUpperBound(0)-1; i++)
                for (int j = 0; j < domains.GetUpperBound(1)-1; j++)
                    for (int v = 1; v < a; v++)
                    {
                        if (s[i, j] * domains[i, j].Right(v, bitmap) != s[i + 1, j] * domains[i + 1, j].Left(v, bitmap))
                            return false;
                        if (s[i, j] * domains[i, j].Up(v, bitmap) != s[i, j + 1] * domains[i, j + 1].Down(v, bitmap))
                            return false;
                    }
            return true;
        }

        public static bool CheckConditionOfContinuity(Domain[,] domains, int i, int j, double[,] s, int a, FractalCompression.Structure.Region region, Bitmap bitmap)
        {
            if (i<0 || j<0 || (i > domains.GetUpperBound(0) - 1) || (j > domains.GetUpperBound(1) - 1))
                throw new ArgumentException("Incorrect i or j values");

            Domain dij = domains[i, j];
            Domain dip1j = domains[i + 1, j];
            Domain dijp1 = domains[i, j + 1];

            double sij = MNTools.ComputeContractivityFactor(dij, region, bitmap);
            double sip1j = MNTools.ComputeContractivityFactor(dip1j, region, bitmap);
            double sijp1 = MNTools.ComputeContractivityFactor(dijp1, region, bitmap);

            for (int v = 1; v < a; v++)
            {
                if (sij * dij.Right(v, bitmap) != sip1j * dip1j.Left(v, bitmap))
                    return false;
                if (sij * dij.Up(v, bitmap) != sijp1 * dijp1.Down(v, bitmap))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns parameters A,B i C of line Ax+By+C=0 crossing points p and r
        /// </summary>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static double[] LineBetweenPointsParameters(PointF p, PointF r)
        {
            double a = r.Y - p.Y, b = r.X - p.X;
            double[] ABC ={ a, b, b * p.Y - a * p.X };
            return ABC;
        }

        public static int DistanceSignum(PointF point, PointF lineStart, PointF lineEnd)
        {
            double[] lineABC = LineBetweenPointsParameters(lineStart, lineEnd);
            double lineVal = (lineABC[0] * point.X + lineABC[2]) / -lineABC[1];
            if (lineVal < point.Y)
                return 1;
            else if (lineVal > point.Y)
                return -1;
            else
                return 0;
        }
    }
}
