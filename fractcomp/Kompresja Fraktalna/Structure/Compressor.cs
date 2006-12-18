using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using FractalCompression.Tools;

namespace FractalCompression.Structure
{
    class Compressor
    {
        private int bigDelta, smallDelta, a, eps, dmax, d;

        private Queue<Region> squeue;
        private Queue<Point> iqueue;

        private Queue<double> cqueue;            //contractivity factors
        private Queue<Point3D> aqueue;          //addresses domain's i,j, minHij
        private Queue<Region> squeue2;

        private double[,] h;
        private Domain[,] domains;
        private Region[,] regions;
        private Bitmap bitmap;
	

        public Compressor(int bigDelta, int a, int eps, int dmax, Domain[,] domains, Region[,] regions, Point[,] interpolationPoints, Bitmap bitmap)
        {
            this.bigDelta = bigDelta;
            this.a = a;
            this.smallDelta = bigDelta / a;
            this.eps = eps;
            this.dmax = dmax;

            this.regions = regions;
            this.domains = domains;

            squeue = new Queue<Region>();
            foreach (Region r in regions)
                squeue.Enqueue(r);

            iqueue = new Queue<Point>();
            foreach (Point p in interpolationPoints)
                iqueue.Enqueue(p);

            aqueue = new Queue<Point3D>();
            cqueue = new Queue<double>();
            squeue2 = new Queue<Region>();

            d = 1;
            this.bitmap = bitmap;

            this.h = new double[domains.GetUpperBound(0), domains.GetUpperBound(1)];
        }

        public void Compress()
        {
            do
            {
                double s = 0;
                while (squeue.Count != 0)
                {
                    Region r = squeue.Dequeue();
                    for (int i = 0; i < domains.GetUpperBound(0); i++)
                        for (int j = 0; j < domains.GetUpperBound(1); j++)
                        {
                            Domain dom = domains[i, j];
                            if (dom != null)
                            {
                                s = MNTools.ComputeContractivityFactor(dom, r, bitmap);
                                if (Math.Abs(s) >= 1)
                                    continue;

                                if (!POTools.CheckConditionOfContinuity(domains, i, j, a, r, bitmap))
                                    continue;

                                Point pk = dom.Vertices[3], pi = r.Vertices[3];
                                Mapper mapper = new Mapper(s, pk, pi, smallDelta, bigDelta,
                                    bitmap.GetPixel(pk.X, pk.Y).ToArgb(), bitmap.GetPixel(pk.X + bigDelta, pk.Y).ToArgb(),
                                    bitmap.GetPixel(pk.X + bigDelta, pk.Y).ToArgb(), bitmap.GetPixel(pk.X + bigDelta, pk.Y + bigDelta).ToArgb(),
                                    bitmap.GetPixel(pk.X, pk.Y).ToArgb(), bitmap.GetPixel(pk.X + smallDelta, pk.Y).ToArgb(),
                                    bitmap.GetPixel(pk.X + smallDelta, pk.Y).ToArgb(), bitmap.GetPixel(pk.X + smallDelta, pk.Y + smallDelta).ToArgb());

                                Region mappedRegion = POTools.MapDomainToRegion(dom, r, bitmap, mapper, a);
                                h[i, j] = MNTools.ComputeDistance(mappedRegion, r, bitmap);
                            }
                        }

                    int minHi = 0, minHj = 0;
                    double minH = h[minHi, minHj];
                    for (int i = 0; i < h.GetUpperBound(0); i++)
                        for (int j = 0; j < h.GetUpperBound(1); j++)
                        {
                            if (h[minHi, minHj] < minH)
                            {
                                minH = h[minHi, minHj];
                                minHi = i;
                                minHj = j;
                            }
                        }

                    if (minH > eps && d < dmax)
                    {
                        //TODO:
                        //create four new regions....
                    }
                    else
                    {
                        //store j with the min distance inside aqueue and s inside cqueue
                        aqueue.Enqueue(new Point3D(minHi, minHj, minH));
                        cqueue.Enqueue(s);
                    }
                }

                if (squeue2.Count != 0)
                {
                    squeue = squeue2;
                    d++;
                }
            } while (squeue2.Count != 0);

            //store dmax, smallDelta, bigDelta, cqueue, iqueue, aqueue
        }

        public Queue<Point> Iqueue
        {
            get { return iqueue; }
        }


        public Queue<Point3D> Aqueue
        {
            get { return aqueue; }
        }

        public Queue<double> Cqueue
        {
            get { return cqueue;}
        }
    }
}
