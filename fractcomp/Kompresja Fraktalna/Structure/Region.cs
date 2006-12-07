using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FractalCompression.Structure
{
    class Region
    {
         private Point[] vertices;

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
            get { return vertices[3].X - vertices[0].X; }
        }

        public Region()
        {
            vertices = new Point[4];
        }

        public Region(Point p0, Point p1, Point p2, Point p3)
        {
            vertices = new Point[4];
            vertices[0] = p0;
            vertices[1] = p1;
            vertices[2] = p2;
            vertices[3] = p3;
        }

        public Region(Point[] vertices)
        {
            if (vertices == null || vertices.Length != 4)
                throw new Exception("Invalid argument");
            this.vertices = vertices;
        }
    }
}
