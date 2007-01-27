using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FractalCompression.Structure;

namespace FractalCompression.Tools
{
    class Decompressor
    {
        private List<double> contractivityFactors;
        private List<MappedPoint> interpolationPoints;
        private List<int> addresses;
        private int smallDelta, bigDelta;
        private int a;
        private int width;
        private int height;
        private int dMax;
        private int realSize;

        public Decompressor(List<double> contrctivtyFactors,
            List<MappedPoint> interpolationPoints, List<int> addresses,
            int smallDelta, int bigDelta, int a, int width, int height, int dMax)
        {
            this.contractivityFactors = contrctivtyFactors;
            this.interpolationPoints = interpolationPoints;
            this.addresses = addresses;
            this.a = a;
            this.smallDelta = smallDelta;
            this.bigDelta = bigDelta;
            this.width = width;
            this.height = height;
            this.dMax = dMax;
            this.realSize = width / smallDelta;
    
        }

        public Bitmap DecompressImage()
        {
            int steps = (int)Math.Truncate(Math.Log(smallDelta, 2) / Math.Log(a, 2));
            Bitmap bit = new Bitmap(width, height, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            foreach (MappedPoint p in interpolationPoints)
            {
                bit.SetPixel(p.X, p.Y,
                        Color.FromArgb((int)p.Val,
                        (int)p.Val,
                        (int)p.Val));
            }
            for (int t = 0; t < steps; t++)
            {
                for (int i = 0; i < Math.Min(steps, dMax); i++)
                {
                    for (int j = 0; j < this.interpolationPoints.Count ; j+=4)
                    {
                        int coresspondingDomain = addresses[j / 4];
                        if (j != -1)
                        {
                            double contractivityFactor = contractivityFactors[j / 4];
                            MyDomain md = GetDomain(coresspondingDomain);
                            Mapper mapper = new Mapper(contractivityFactor,
                                md.Domain.Vertices[0], interpolationPoints[j],
                                this.smallDelta, this.bigDelta, md.Vals[0],
                                md.Vals[1], md.Vals[2], md.Vals[3],
                                (int)interpolationPoints[j].Val,
                                (int)interpolationPoints[j + 1].Val,
                                (int)interpolationPoints[j + 2].Val,
                                (int)interpolationPoints[j + 3].Val);
                            Point prevPoint = md.Domain.Vertices[0];
                            int prevVal = md.Vals[0];
                            for (int k = 0; k < md.Domain.Size * md.Domain.Size; k++)
                            {
                                MappedPoint newPoint = mapper.MapPoint(prevPoint.X,
                                    prevPoint.Y, (double)prevVal);
                                bit.SetPixel(newPoint.X, newPoint.Y,
                                    Color.FromArgb((int)newPoint.Val,
                                    (int)newPoint.Val, (int)newPoint.Val));
                                prevPoint = newPoint;
                                prevVal = (int)newPoint.Val;
                            }
                        }
                    }
                }
            }
            return bit;
        }

        private MyDomain GetDomain(int address)
        {
            int numberOfDomainInWidth = this.width / this.bigDelta;
            int numberOfDomainInHeight = this.height / this.bigDelta;
            int domainX = (address / numberOfDomainInWidth) * this.bigDelta;
            int domainY = (address % numberOfDomainInHeight) * this.bigDelta;
            MyDomain md = null;
            for (int i = 0; i < this.interpolationPoints.Count; i++)
            {
                if (domainX == interpolationPoints[i].X &&
                    domainY == interpolationPoints[i].Y)
                {
                    int[] vals = new int[4];
                    Domain d = new Domain(interpolationPoints[i],
                        interpolationPoints[i + 1],
                        interpolationPoints[i + 2],
                        interpolationPoints[i + 3], this.a);
                    vals[0] = (int)interpolationPoints[i].Val;
                    vals[1] = (int)interpolationPoints[i + 1].Val;
                    vals[2] = (int)interpolationPoints[i + 2].Val;
                    vals[3] = (int)interpolationPoints[i + 3].Val;
                    md = new MyDomain(d, vals);
                    break;
                }
                i += 4;
            }
            return md;
        }

        private class MyDomain
        {
            private int[] vals = new int[4];
            private Domain domain;

            public Domain Domain
            {
                get { return domain; }
            }

            public int[] Vals
            {
                get { return vals; }
            }

            public MyDomain(Domain domain, int[] vals)
            {
                this.domain = domain;
                this.vals = vals;
            }

        }
    }
}
