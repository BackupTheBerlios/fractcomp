using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FractalCompression.Structure
{
    class SimpledRegion
    {
        private Point[] vertices;
        private int[] vals;

        //0 - (x0,y0)  1 - (x0,y1), 2 - (x1,y1), 3 - (x1, y0)
        public static SimpledRegion CreateSimpledRegion(Region region)
        {
            SimpledRegion sr = new SimpledRegion();
            sr.Vertices = region.Vertices;
            for (int i = 0; i < 4; i++)
                sr.vals[i] = (int)Math.Round(region[region.Vertices[i]]);
            return sr;
        }
        
        public int[] Values
        {
            get { return vals; }
            set
            {
                if (value == null || value.Length != 4)
                    throw new Exception("Invalid argument");
                vals = value;
            }
        }

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

        private SimpledRegion()
        {
            Point[] vertices = new Point[4];
            int[] vals = new int[4];
        }

       
    }
}
