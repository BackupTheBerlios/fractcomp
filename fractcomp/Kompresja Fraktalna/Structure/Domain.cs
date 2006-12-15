using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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


    }
}
