using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

using FractalCompression.Structure;

namespace FractalCompression.Tools
{
    class Compressor
    {
        private int bigDelta, smallDelta, a, eps, dmax, d;

        private Queue<FractalCompression.Structure.Region> squeue;
        private Queue<Point3D> iqueue;

        private Queue<double> cqueue;               //contractivity factors
        //private Queue<Point3D> aqueue;              //addresses domain's i,j, minHij
        private Queue<int> aqueue;                     //addresses domain's
        private Queue<FractalCompression.Structure.Region> squeue2;

        private double[,] h;
        private Domain[,] domains;
        private FractalCompression.Structure.Region[,] regions;
        private Bitmap bitmap;


        public Compressor(int bigDelta, int a, int eps, int dmax, Domain[,] domains, FractalCompression.Structure.Region[,] regions, Point[,] interpolationPoints, Bitmap bitmap)
        {
            this.bigDelta = bigDelta;
            this.a = a;
            this.smallDelta = bigDelta / a;
            this.eps = eps;
            this.dmax = dmax;

            this.regions = regions;
            this.domains = domains;

            squeue = new Queue<FractalCompression.Structure.Region>();
            foreach (FractalCompression.Structure.Region r in regions)
                squeue.Enqueue(r);

            iqueue = new Queue<Point3D>();
            foreach (Point p in interpolationPoints)
                iqueue.Enqueue(new Point3D(p.X, p.Y, bitmap.GetPixel(p.X,p.Y).ToArgb()));

            //aqueue = new Queue<Point3D>();
            aqueue = new Queue<int>();
            cqueue = new Queue<double>();
            squeue2 = new Queue<FractalCompression.Structure.Region>();

            d = 1;
            this.bitmap = bitmap;

            this.h = new double[regions.Length, domains.Length];
        }

        public void Compress()
        {
            do
            {
                double s = 0;
                int rCount = -1;
                double[] nh = new double[domains.Length];
                while (squeue.Count != 0)
                {
                    FractalCompression.Structure.Region r = squeue.Dequeue();
                    rCount++;

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

                                FractalCompression.Structure.Region mappedRegion = POTools.MapDomainToRegion(dom, r, bitmap, mapper, a);
                                
                                // to jest Ÿle - hij odnosi siê do j-tej domeny i i-tego regionu!
                                //h[i, j] = MNTools.ComputeDistance(mappedRegion, r, bitmap);

                                nh[domains.GetUpperBound(0) * i + j] = MNTools.ComputeDistance(mappedRegion, r, bitmap);
                            }
                        }

                    /*int minHi = 0, minHj = 0;
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
                     */

                    int minHj = 0;
                    double minH = nh[0];
                    for (int j = 0; j < nh.Length; j++)
                        {
                            if (nh[minHj] < minH)
                            {
                                minH = nh[minHj];
                                minHj = j;
                            }
                        }

                    if (minH > eps && d < dmax)
                    {
                        int hx = (r.Vertices[1].X + r.Vertices[2].X) / 2;
                        int hy = (r.Vertices[2].Y + r.Vertices[3].Y) / 2;

                        Point pN = new Point(hx, r.Vertices[1].Y);
                        Point pE = new Point(r.Vertices[3].X, hy);
                        Point pS = new Point(hx, r.Vertices[0].Y);
                        Point pW = new Point(r.Vertices[0].X, hy);
                        Point pC = new Point(hx, hy);

                        squeue2.Enqueue(new FractalCompression.Structure.Region(r.Vertices[0], pS, pC, pW));
                        squeue2.Enqueue(new FractalCompression.Structure.Region(pS, pC, pE, r.Vertices[3]));
                        squeue2.Enqueue(new FractalCompression.Structure.Region(pC, pN, r.Vertices[2], pE));
                        squeue2.Enqueue(new FractalCompression.Structure.Region(pW, r.Vertices[1], pN, pC));

                        /*iqueue.Enqueue(pN);
                        iqueue.Enqueue(pE);
                        iqueue.Enqueue(pS);
                        iqueue.Enqueue(pW);
                        iqueue.Enqueue(pC);*/

                        iqueue.Enqueue(new Point3D(pN.X, pN.Y, bitmap.GetPixel(pN.X, pN.Y).ToArgb()));
                        iqueue.Enqueue(new Point3D(pE.X, pE.Y, bitmap.GetPixel(pE.X, pE.Y).ToArgb()));
                        iqueue.Enqueue(new Point3D(pS.X, pS.Y, bitmap.GetPixel(pS.X, pS.Y).ToArgb()));
                        iqueue.Enqueue(new Point3D(pW.X, pW.Y, bitmap.GetPixel(pW.X, pW.Y).ToArgb()));
                        iqueue.Enqueue(new Point3D(pC.X, pC.Y, bitmap.GetPixel(pC.X, pC.Y).ToArgb()));

                        aqueue.Enqueue(-1);
                    }
                    else
                    {
                        //store j with the min distance inside aqueue and s inside cqueue
                        //aqueue.Enqueue(new Point3D(minHi, minHj, minH));
                        aqueue.Enqueue(minHj);
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

        public void SaveToFile(String filepath)
        {
            CompResult results = new CompResult(aqueue, cqueue, iqueue, bigDelta, smallDelta, dmax);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filepath, FileMode.Create);
            bf.Serialize(fs, results);
            fs.Close();
        }

        public Queue<Point3D> Iqueue
        {
            get { return iqueue; }
        }


        /*public Queue<Point3D> Aqueue
        {
            get { return aqueue; }
        }*/

        public Queue<int> Aqueue
        {
            get { return aqueue; }
        }

        public Queue<double> Cqueue
        {
            get { return cqueue;}
        }

        public int BigDelta
        {
            get { return bigDelta; }
        }

        public int SmallDelta
        {
            get { return smallDelta; }
        }
    }
}
