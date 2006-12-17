using System;
using System.Collections.Generic;
using System.Text;

namespace FractalCompression.Tools
{
    class Point3D
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
