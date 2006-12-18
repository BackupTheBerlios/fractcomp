using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FractalCompression.Structure
{
    class Region
    {
        private Point[] vertices;
        private double[,] mappedVals;

        public Point[] Vertices
        {
            get { return vertices; }
            set {
                if (value == null || value.Length != 4)
                    throw new Exception("Invalid argument");
                vertices = value; }
        }

        public double this[int x, int y]  //indexer
        {
            get
            {
                if (x >= 0 && x <=Size && y>=0 && y<=Size)
                {
                    return mappedVals[x,y];
                }
                else
                {
                    throw new System.IndexOutOfRangeException();
                }
            }
            set
            {
                if (x >= 0 && x <= Size && y >= 0 && y <= Size)
                {
                   mappedVals[x,y] = value;
                }
                else
                {
                    throw new System.IndexOutOfRangeException();
                }
            }
        }

        public double[,] MappedVals
        {
            get { return mappedVals; }
            set { mappedVals = value; }
        }

        public int Size
        {
            get { return vertices[3].X - vertices[0].X; }
        }

        public Region(Point p0, Point p1, Point p2, Point p3)
        {
            vertices = new Point[4];
            vertices[0] = p0;
            vertices[1] = p1;
            vertices[2] = p2;
            vertices[3] = p3;
            mappedVals = new double[p3.X - p0.X + 1, p1.Y - p0.Y + 1];
        }

        public Region(Point[] vertices)
        {
            if (vertices == null || vertices.Length != 4)
                throw new Exception("Invalid argument");
            this.vertices = vertices;
            mappedVals = new double[vertices[3].X - vertices[0].X + 1, vertices[1].Y - vertices[0].Y + 1];
        }
    }
}
