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

        public class Point3D
        {
            private double x, y, z;

            public Point3D(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public double Z
            {
                get { return z; }
                set { z = value; }
            }

            public double Y
            {
                get { return y; }
                set { y = value; }
            }

            public double X
            {
                get { return x; }
                set { x = value; }
            }

            private static double Magnitude(Point3D point1, Point3D point2)
            {
                Point3D vector = new Point3D(
                    point2.X - point1.X,
                    point2.Y - point1.Y,
                    point2.Z - point1.Z);

                return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            }

            public static double DistancePointLine(Point3D point, Point3D lineStart, Point3D lineEnd)
            {
                double lineMag = Magnitude(lineEnd, lineStart);

                double u = (((point.X - lineStart.X) * (lineEnd.X - lineStart.X)) +
                    ((point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) +
                    ((point.Z - lineStart.Z) * (lineEnd.Z - lineStart.Z))) /
                    (lineMag * lineMag);

                if (u < 0.0f || u > 1.0f)
                    return -1;   // closest point does not fall within the line segment

                Point3D intersection = new Point3D(
                    lineStart.X + u * (lineEnd.X - lineStart.X),
                    lineStart.Y + u * (lineEnd.Y - lineStart.Y),
                    lineStart.Z + u * (lineEnd.Z - lineStart.Z));

                return Magnitude(point, intersection);
            }
        }
    }
}
