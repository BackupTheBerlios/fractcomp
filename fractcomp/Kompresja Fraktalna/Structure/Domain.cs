using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using FractalCompression.Tools;

namespace FractalCompression.Structure
{
    class Domain
    {
        //0 - (x0,y0)  1 - (x0,y1), 2 - (x1,y1), 3 - (x1, y0)
        private Point[] vertices;
        private int bigDelta;
        private int smallDelta;
        private int a;
        
        public Point[] Vertices
        {
            get { return vertices; }
            set {
                if (value == null || value.Length != 4)
                    throw new Exception("Invalid argument");
                vertices = value; }
        }

        public int Size
        {
            get { return bigDelta; }
        }

        public Domain()
        {
            vertices = new Point[4];
        }

        public Domain(Point p0, Point p1, Point p2, Point p3, int a)
        {
            vertices = new Point[4];
            vertices[0] = p0;
            vertices[1] = p1;
            vertices[2] = p2;
            vertices[3] = p3;
            bigDelta = vertices[3].X - vertices[0].X;
            this.a = a;
            this.smallDelta = bigDelta / a;
        }

        public Domain(Point[] vertices, int a)
        {
            if (vertices == null || vertices.Length != 4)
                throw new Exception("Invalid argument");
            this.vertices = vertices;
            bigDelta = vertices[3].X - vertices[0].X;
            this.a = a;
            this.smallDelta = bigDelta / a;
        }

        public double Left(int v, Bitmap bitmap)
        {
            Point3D interpolP = new Point3D(
                vertices[0].X,
                vertices[0].Y + a * v,
                bitmap.GetPixel(vertices[0].X, vertices[0].Y + a * v).ToArgb());

            Point3D lineStart = new Point3D(
                vertices[0].X,
                vertices[0].Y,
                bitmap.GetPixel(vertices[0].X, vertices[0].Y).ToArgb());

            Point3D lineEnd = new Point3D(
                vertices[1].X,
                vertices[1].Y,
                bitmap.GetPixel(vertices[1].X, vertices[1].Y).ToArgb());

            int signum = POTools.DistanceSignum(
                new PointF((float)interpolP.Y, (float)interpolP.Z),
                new PointF((float)lineStart.Y, (float)lineStart.Z),
                new PointF((float)lineEnd.Y, (float)lineEnd.Z));

            return signum * Point3D.DistancePointLine(interpolP, lineStart, lineEnd);
        }

        public double Right(int v, Bitmap bitmap)
        {
            Point3D interpolP = new Point3D(
                vertices[3].X,
                vertices[3].Y + a * v,
                bitmap.GetPixel(vertices[3].X, vertices[3].Y + a * v).ToArgb());

            Point3D lineStart = new Point3D(
                vertices[2].X,
                vertices[2].Y,
                bitmap.GetPixel(vertices[2].X, vertices[2].Y).ToArgb());

            Point3D lineEnd = new Point3D(
                vertices[3].X,
                vertices[3].Y,
                bitmap.GetPixel(vertices[3].X, vertices[3].Y).ToArgb());

            int signum = POTools.DistanceSignum(
                new PointF((float)interpolP.Y, (float)interpolP.Z),
                new PointF((float)lineStart.Y, (float)lineStart.Z),
                new PointF((float)lineEnd.Y, (float)lineEnd.Z));

            return signum * Point3D.DistancePointLine(interpolP, lineStart, lineEnd);
        }

        public double Down(int v, Bitmap bitmap)
        {
            Point3D interpolP = new Point3D(
                vertices[0].X + a * v,
                vertices[0].Y,
                bitmap.GetPixel(vertices[0].X + a * v, vertices[0].Y).ToArgb());

            Point3D lineStart = new Point3D(
                vertices[0].X,
                vertices[0].Y,
                bitmap.GetPixel(vertices[0].X, vertices[0].Y).ToArgb());

            Point3D lineEnd = new Point3D(
                vertices[3].X,
                vertices[3].Y,
                bitmap.GetPixel(vertices[3].X, vertices[3].Y).ToArgb());

            int signum = POTools.DistanceSignum(
               new PointF((float)interpolP.X, (float)interpolP.Z),
               new PointF((float)lineStart.X, (float)lineStart.Z),
               new PointF((float)lineEnd.X, (float)lineEnd.Z));

            return signum * Point3D.DistancePointLine(interpolP, lineStart, lineEnd);
        }

        public double Up(int v, Bitmap bitmap)
        {
            Point3D interpolP = new Point3D(
                vertices[1].X + a * v,
                vertices[1].Y,
                bitmap.GetPixel(vertices[1].X + a * v, vertices[1].Y).ToArgb());

            Point3D lineStart = new Point3D(
                vertices[1].X,
                vertices[1].Y,
                bitmap.GetPixel(vertices[1].X, vertices[1].Y).ToArgb());

            Point3D lineEnd = new Point3D(
                vertices[2].X,
                vertices[2].Y,
                bitmap.GetPixel(vertices[2].X, vertices[2].Y).ToArgb());

            int signum = POTools.DistanceSignum(
               new PointF((float)interpolP.X, (float)interpolP.Z),
               new PointF((float)lineStart.X, (float)lineStart.Z),
               new PointF((float)lineEnd.X, (float)lineEnd.Z));

            return signum * Point3D.DistancePointLine(interpolP, lineStart, lineEnd);
        }
    }
}
