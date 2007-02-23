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
        private int goodPixel = 0;
        private int badPixel = 0;
        private int interPixel = 0;
        private double tempS = 0;
        private int tempAdd = 0;
        int numberOfDomainInWidth = 0;
        int numberOfDomainInHeight = 0;

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
            this.numberOfDomainInWidth = this.width / this.bigDelta;
            this.numberOfDomainInHeight = this.height / this.bigDelta;
        }

        public Bitmap DecompressImage(out Bitmap bitmapWithGrid)
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
                interPixel++;
            }
            for (int t = 0; t < steps; t++)
            {
                for (int i = 0; i < Math.Min(steps, dMax); i++)
                {
                    for (int j = 0; j < this.interpolationPoints.Count; j += 4)
                    {
                        int coresspondingDomain = addresses[j / 4];
                        if (coresspondingDomain != -1)
                        {
                            double contractivityFactor = contractivityFactors[j / 4];
                            MyDomain md = FindDomainByAddress(coresspondingDomain);
                            Mapper mapper = new Mapper(contractivityFactor,
                                md.Domain.Vertices[1], interpolationPoints[j + 1],
                                this.smallDelta, this.bigDelta,
                                 md.Vals[0], md.Vals[1], md.Vals[2], md.Vals[3],
                                (int)interpolationPoints[j].Val,
                                 (int)interpolationPoints[j + 1].Val,
                                (int)interpolationPoints[j + 2].Val,
                                (int)interpolationPoints[j + 3].Val,
                                 false);
                            Point[] prevPoint = md.Domain.Vertices;
                            int[] prevVal = md.Vals;
                            for (int k = 0; k < md.Domain.Size * md.Domain.Size; k++)
                            {
                                MappedPoint[] newPoint = new MappedPoint[prevPoint.Length];
                                for (int pp = 1; pp <= 1; pp++)
                                {
                                    newPoint[pp] = mapper.MapPoint(prevPoint[pp].X,
                                         prevPoint[pp].Y, (double)prevVal[pp]);
                                    Structure.Region tempRegion = FindRegion(newPoint[pp]);
                                    if (tempAdd != -1 && tempS != -1)
                                    {
                                        md = FindDomainByAddress(tempAdd);
                                        mapper = new Mapper(tempS, md.Domain.Vertices[1], tempRegion.Vertices[1],
                                             this.smallDelta, this.bigDelta,
                                             md.Vals[0], md.Vals[1], md.Vals[2], md.Vals[3],
                                            (int)tempRegion[0, tempRegion.Size],
                                            (int)tempRegion[0, 0],
                                            (int)tempRegion[tempRegion.Size, 0],
                                            (int)tempRegion[tempRegion.Size, tempRegion.Size],
                                            true);
                                        Console.Out.WriteLine(md.Domain.Vertices[1].X + " " + md.Domain.Vertices[1].Y
                                        + " " + md.Domain.Vertices[2].X + " " + md.Domain.Vertices[2].Y
                                        + " " + md.Domain.Vertices[3].X + " " + md.Domain.Vertices[3].Y
                                        + " " + md.Domain.Vertices[0].X + " " + md.Domain.Vertices[0].Y);
                                    }
                                    SafePutPixel(newPoint[pp].X, newPoint[pp].Y, (int)newPoint[pp].Val
                                                 , bit);
                                    prevPoint[pp] = newPoint[pp];
                                    prevVal[pp] = (int)newPoint[pp].Val;
                                }
                            }
                        }
                    }
                }
            }
            Console.Out.WriteLine(goodPixel + " " + badPixel);
            bitmapWithGrid = (Bitmap)bit.Clone();
            for (int i = 0; i < interpolationPoints.Count; i += 4)
            {
                for (int k = interpolationPoints[i + 1].Y; k < interpolationPoints[i].Y; k++)
                {
                    bitmapWithGrid.SetPixel(interpolationPoints[i + 1].X, k, Color.Red);
                    bitmapWithGrid.SetPixel(interpolationPoints[i + 2].X, k, Color.Red);
                }
                for (int k = interpolationPoints[i + 1].X; k < interpolationPoints[i + 2].X; k++)
                {
                    bitmapWithGrid.SetPixel(k, interpolationPoints[i + 1].Y, Color.Red);
                    bitmapWithGrid.SetPixel(k, interpolationPoints[i].Y, Color.Red);
                }
            }
            return bit;
        }

        private Structure.Region FindRegion(MappedPoint point)
        {
            for (int i = 0; i < interpolationPoints.Count; i += 4)
            {
                if (point.X >= interpolationPoints[i].X && point.X <= interpolationPoints[i + 2].X &&
                    point.Y >= interpolationPoints[i + 1].Y && point.Y <= interpolationPoints[i].Y)
                {
                    this.tempAdd = addresses[i / 4];
                    this.tempS = contractivityFactors[i / 4];
                    MappedPoint[] points = new MappedPoint[]{ interpolationPoints[i], 
                        interpolationPoints[i+1],interpolationPoints[i+2], interpolationPoints[i+3]};
                    Structure.Region region = new FractalCompression.Structure.Region(points);
                    region[0, region.Size] = interpolationPoints[0].Val;
                    region[0, 0] = interpolationPoints[1].Val;
                    region[region.Size, 0] = interpolationPoints[2].Val;
                    region[region.Size, region.Size] = interpolationPoints[3].Val;
                    return region;
                }
            }
            tempS = -1;
            tempAdd = -1;
            return null;
        }

        private MappedPoint[] ConvertMappedPoint(MappedPoint mapPoint)
        {
            MappedPoint[] mappedPoints = new MappedPoint[4];
            mappedPoints[0] = new MappedPoint((int)mapPoint.X, (int)mapPoint.Y, mapPoint.Val);
            mappedPoints[1] = new MappedPoint((int)mapPoint.X + 1, (int)mapPoint.Y, mapPoint.Val);
            mappedPoints[2] = new MappedPoint((int)mapPoint.X, (int)mapPoint.Y + 1, mapPoint.Val);
            mappedPoints[3] = new MappedPoint((int)mapPoint.X + 1, (int)mapPoint.Y + 1, mapPoint.Val);
            return mappedPoints;
        }

        private bool SafePutPixel(int x, int y, int val, Bitmap bit)
        {
            if (x >= 0 && y >= 0 && x < bit.Width && y < bit.Width && val < 256 && val >= 0)
            {
                if (bit.GetPixel(x, y).ToArgb()==Color.Green.ToArgb())
                {
                    bit.SetPixel(x, y, Color.FromArgb(val, val, val));
                    goodPixel++;
                    return true;
                }
                return false;
            }
            else 
            {
                badPixel++;
                return false;
            }
        }

        private Queue<MappedPoint>[] PrepareLists()
        {
            //przygotowanie listy list punktow
            Queue<MappedPoint>[] domainsPoints = new Queue<MappedPoint>[numberOfDomainInWidth*numberOfDomainInHeight];
            for (int i = 0; i < domainsPoints.Length; i++)
                domainsPoints[i] = new Queue<MappedPoint>();
            for (int i = 0; i < interpolationPoints.Count; i += 4)
            {
                //if (addresses[i / 4] != -1)
                {
                    int counter = FindDomainByPoint(interpolationPoints[i + 1]);
                    if(counter != -1)

                        for (int j = i; j < i + 4; j++)
                        {
                            domainsPoints[counter].Enqueue(interpolationPoints[j]);
                        }
                }
            }
            return domainsPoints;
        }

        private int FindDomainByPoint(Point point)
        {
            int counter = -1;
            int domainX = 0;
            int domainY = 0;
            for (counter = 0; counter < numberOfDomainInWidth * numberOfDomainInHeight; counter++)
            {
                if (point.X >= domainX && point.X <= domainX + bigDelta - 1
                    && point.Y >= domainY && point.Y <= domainY + bigDelta - 1)
                {
                    return counter;
                }
                domainX += bigDelta;
                if (domainX >= this.width)
                {
                    domainX = 0;
                    domainY += bigDelta;
                }
            }
            return -1;
        }
        
        public Bitmap DecompressImageVerII(out Bitmap bitmapWithGrid)
        {
            Bitmap bit = new Bitmap(width, height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Queue<MappedPoint>[] domainsPoints = PrepareLists();
            Graphics.FromImage(bit).FillRectangle(Brushes.Green, 0, 0, this.width, this.height);
            
            //punkty interpolacyjne
            foreach (MappedPoint p in interpolationPoints)
            {
                bit.SetPixel(p.X, p.Y,
                        Color.FromArgb((int)p.Val,
                        (int)p.Val,
                        (int)p.Val));
                interPixel++;
            }

            
            //g³owna petla
            bool newPixel = true;
            int cos=0;
            Random rand = new Random();
            while (newPixel)
          //  while(cos<10)
            {
                newPixel = false;
                Console.Out.WriteLine("Iteration " + cos++);

                for (int i = 0; i < interpolationPoints.Count; i+=4)
                {
                    int address = addresses[i / 4];
                    double s = contractivityFactors[i / 4];
                   // address = rand.Next(numberOfDomainInHeight * numberOfDomainInWidth);
                   // s = rand.NextDouble();
                    if (address != -1)                     
                    {
                        MyDomain domain = FindDomainByAddress(address);
                        Mapper mapper = new Mapper(s, domain.Domain.Vertices[1],
                            interpolationPoints[i + 1], smallDelta, bigDelta,
                            domain.Vals[0], domain.Vals[1], domain.Vals[2], domain.Vals[3],
                            (int)interpolationPoints[i].Val, (int)interpolationPoints[i + 1].Val,
                            (int)interpolationPoints[i + 2].Val, (int)interpolationPoints[i + 3].Val, false);
                        int sizeOfQueue = domainsPoints[address].Count;
                        for (int j = 0; j < sizeOfQueue; j++)
                        {
                            
                            MappedPoint point = domainsPoints[address].Dequeue();
                            domainsPoints[address].Enqueue(point);
                            MappedPoint newPoint = mapper.MapPoint(point.X, point.Y, point.Val);
                            MappedPoint[] mapPoints = ConvertMappedPoint(newPoint);
                            for(int k=0;k<mapPoints.Length;k++)
                                if (SafePutPixel(mapPoints[k].X, mapPoints[k].Y, (int)newPoint.Val, bit))
                            {
                                newPixel = true;
                                int domainAddress = FindDomainByPoint(mapPoints[k]);
                                domainsPoints[domainAddress].Enqueue(mapPoints[k]);
                            }
                        }

                    }
                }
            }
            
            //tworzenie siatki
            bitmapWithGrid = (Bitmap)bit.Clone();
            for (int i = 0; i < interpolationPoints.Count; i += 4)
            {
                for (int k = interpolationPoints[i + 1].Y; k < interpolationPoints[i].Y; k++)
                {
                    bitmapWithGrid.SetPixel(interpolationPoints[i + 1].X, k, Color.Red);
                    bitmapWithGrid.SetPixel(interpolationPoints[i + 2].X, k, Color.Red);
                }
                for (int k = interpolationPoints[i + 1].X; k < interpolationPoints[i + 2].X; k++)
                {
                    bitmapWithGrid.SetPixel(k, interpolationPoints[i + 1].Y, Color.Red);
                    bitmapWithGrid.SetPixel(k, interpolationPoints[i].Y, Color.Red);
                }
            }
            Console.Out.WriteLine(goodPixel + " " + badPixel+" "+interPixel);
            return bit;
        }

        private MyDomain FindDomainByAddress(int address)
        {
            int domainX = (address % numberOfDomainInWidth) * this.bigDelta;
            int domainY = (address / numberOfDomainInHeight) * this.bigDelta;
            MyDomain md = null;
            for (int i = 0; i < this.interpolationPoints.Count; i += 4)
            {
                if (domainX == interpolationPoints[i + 1].X &&
                    domainY == interpolationPoints[i + 1].Y)
                {
                    int[] vals = new int[4];
                    Domain d = new Domain(new Point(interpolationPoints[i + 1].X, interpolationPoints[i + 1].Y + bigDelta - 1),
                        interpolationPoints[i + 1],
                        new Point(interpolationPoints[i + 1].X + bigDelta - 1, interpolationPoints[i + 1].Y),
                        new Point(interpolationPoints[i + 1].X + bigDelta - 1, interpolationPoints[i + 1].Y + bigDelta - 1), this.a);
                    vals[0] = (int)interpolationPoints[i + 4 * a * numberOfDomainInWidth - 1].Val;
                    vals[1] = (int)interpolationPoints[i + 1].Val;
                    vals[2] = (int)interpolationPoints[i + 6].Val;
                    vals[3] = (int)interpolationPoints[i + 4 * a * numberOfDomainInWidth + 6].Val;
                    md = new MyDomain(d, vals);
                    break;
                }
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
