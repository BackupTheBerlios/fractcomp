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
        private Queue<MappedPoint> iqueue;

        private Queue<double> cqueue;               //contractivity factors
        //private Queue<Point3D> aqueue;              //addresses domain's i,j, minHij
        private Queue<int> aqueue;                     //addresses domain's
        private Queue<FractalCompression.Structure.Region> squeue2;

        private double[,] h;
        private Domain[,] domains;
        private FractalCompression.Structure.Region[,] regions;
        private Bitmap bitmap;


        public Compressor(int bigDelta, int a, int eps, int dmax, Domain[,] domains, FractalCompression.Structure.Region[,] regions, List<MappedPoint> interpolationPoints, Bitmap bitmap)
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

            iqueue = new Queue<MappedPoint>();
            foreach (MappedPoint mp in interpolationPoints)
                if (!iqueue.Contains(mp))
                    iqueue.Enqueue(mp);

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

                               Point pk = dom.Vertices[1], pi = r.Vertices[1];
                                Mapper mapper = new Mapper(s, pk, pi, smallDelta, bigDelta,
                                    MNTools.GetBitmapValue(pk.X, pk.Y, bitmap), 
                                    MNTools.GetBitmapValue(pk.X + bigDelta - 1, pk.Y, bitmap),
                                    MNTools.GetBitmapValue(pk.X, pk.Y + bigDelta - 1, bitmap), 
                                    MNTools.GetBitmapValue(pk.X + bigDelta - 1, pk.Y + bigDelta - 1, bitmap),
                                    MNTools.GetBitmapValue(pi.X, pi.Y, bitmap), 
                                    MNTools.GetBitmapValue(pi.X + smallDelta, pi.Y, bitmap),
                                    MNTools.GetBitmapValue(pi.X , pi.Y + smallDelta - 1, bitmap), 
                                    MNTools.GetBitmapValue(pi.X + smallDelta - 1, pi.Y + smallDelta - 1, bitmap));

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

                    //for(int j=0; j<nh.Length; ++j)
                    //    Console.Write(nh[j]+" ");

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

                        iqueue.Enqueue(new MappedPoint(pN.X, pN.Y, MNTools.GetBitmapValue(pN.X, pN.Y, bitmap)));
                        iqueue.Enqueue(new MappedPoint(pE.X, pE.Y, MNTools.GetBitmapValue(pE.X, pE.Y, bitmap)));
                        iqueue.Enqueue(new MappedPoint(pS.X, pS.Y, MNTools.GetBitmapValue(pS.X, pS.Y, bitmap)));
                        iqueue.Enqueue(new MappedPoint(pW.X, pW.Y, MNTools.GetBitmapValue(pW.X, pW.Y, bitmap)));
                        iqueue.Enqueue(new MappedPoint(pC.X, pC.Y, MNTools.GetBitmapValue(pC.X, pC.Y, bitmap)));

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
            CompResult results = new CompResult(aqueue, cqueue, iqueue, bigDelta, smallDelta, a, dmax,bitmap.Width,bitmap.Height);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filepath, FileMode.Create);
            bf.Serialize(fs, results);
            fs.Close();
        }

        public Queue<MappedPoint> Iqueue
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
