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
        private Queue<int> aqueue;                     //addresses domain's
        private Queue<FractalCompression.Structure.Region> squeue2;

        private List<double> minHQueue;
        private List<int> optimizedAQueue = null;

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
            this.minHQueue = new List<double>();
        }

        public void Compress()
        {
            int contErr = 0, divErr=0;
            do
            {
                double s = 0;
                double[] nh = new double[domains.Length];

                Random rand = new Random();
                while (squeue.Count != 0)
                {
                    FractalCompression.Structure.Region r = squeue.Dequeue();
                    for (int i = 0; i < nh.Length; ++i)
                        nh[i] = -1;

                    for (int i = 0; i <= domains.GetUpperBound(0); i++)
                        for (int j = 0; j <= domains.GetUpperBound(1); j++)
                        {
                            Domain dom = domains[i, j];
                            if (dom != null)
                            {
                                s = MNTools.ComputeContractivityFactor(dom, r, bitmap);
                                //s = rand.NextDouble();

                                if (Math.Abs(s) >= 1)
                                {
                                    //aqueue.Enqueue(-1);
                                    //cqueue.Enqueue(-1);
                                    contErr++;
                                    //Console.WriteLine("csErr: " + contErr++);
                                    continue;
                                }
                                divErr++;


                                if(i!=domains.GetUpperBound(0) && j!=domains.GetUpperBound(1))
                                    if (!POTools.CheckConditionOfContinuity(domains, i, j, a, r, bitmap))
                                        continue;

                               Point pk = dom.Vertices[1], pi = r.Vertices[1];
                                Mapper mapper = new Mapper(s, pk, pi, smallDelta, bigDelta,
                                    MNTools.GetBitmapValue(pk.X, pk.Y, bitmap), 
                                    MNTools.GetBitmapValue(pk.X + bigDelta - 1, pk.Y, bitmap),
                                    MNTools.GetBitmapValue(pk.X, pk.Y + bigDelta - 1, bitmap), 
                                    MNTools.GetBitmapValue(pk.X + bigDelta - 1, pk.Y + bigDelta - 1, bitmap),
                                    MNTools.GetBitmapValue(pi.X, pi.Y, bitmap), 
                                    MNTools.GetBitmapValue(pi.X + r.Size, pi.Y, bitmap),
                                    MNTools.GetBitmapValue(pi.X , pi.Y + r.Size, bitmap), 
                                    MNTools.GetBitmapValue(pi.X + r.Size, pi.Y + r.Size, bitmap), false);

                                FractalCompression.Structure.Region mappedRegion = POTools.MapDomainToRegion(dom, r, bitmap, mapper, a);
                                nh[(domains.GetUpperBound(0)+1) * i + j] = MNTools.ComputeDistance(mappedRegion, r, bitmap);
                                //Console.WriteLine("nh[{0}]={1}", domains.GetUpperBound(0) * i + j, nh[domains.GetUpperBound(0) * i + j]);
                            }
                        }

                    /*for (int j = 0; j < nh.Length; j++)
                        Console.Write(nh[j] + " ");
                    Console.WriteLine();*/

                    int minHj = 0;
                    double minH = nh[0];
                    for (int j = 0; j < nh.Length; j++)
                    {
                        if (nh[j] >= 0)
                        {
                            minHj = j;
                            minH = nh[j];
                            break;
                        }
                    }

                    for (int j = minHj + 1; j < nh.Length; j++)
                    {
                        if (nh[j] < minH && nh[j] >= 0)
                        {
                            minH = nh[j];
                            minHj = j;
                        }
                    }
                    //Console.WriteLine("minH = {0} - nh[{1}]", minH, minHj);

                    if ((minH > eps || minH==-1) && d < dmax)
                    {
                        DivideRegion(r);
                        aqueue.Enqueue(-1);
                        cqueue.Enqueue(-1);
                        //Console.WriteLine("divErr: " + divErr++);
                        minHQueue.Add(minH);
                    }
                    else if (minH >= 0)   //store j with the min distance inside aqueue and s inside cqueue
                    {
                        //Console.WriteLine("minHj: {0}, s: {1}", minHj, s);
                        /*for (int j = 0; j < nh.Length; j++)
                            Console.Write((int)nh[j] + " ");
                        Console.WriteLine();*/
                        
                        aqueue.Enqueue(minHj);
                        cqueue.Enqueue(s);
                        minHQueue.Add(minH);
                    }
                 //   else
                   //     Console.WriteLine("uups");
                }

                if (squeue2.Count != 0)
                {
                    squeue = squeue2;
                    d++;
                }
            } while (squeue.Count != 0);
            Console.WriteLine(contErr);
            Console.WriteLine(divErr);
            //this.optimizedAQueue = POTools.OptimizeAdressesList(aqueue, minHQueue, domains.Length - 1);
        }

        private void DivideRegion(FractalCompression.Structure.Region r)
        {
            int hx = (r.Vertices[1].X + r.Vertices[2].X) / 2;
            int hy = (r.Vertices[2].Y + r.Vertices[3].Y) / 2;

            Point pN = new Point(hx, r.Vertices[1].Y);
            Point pE = new Point(r.Vertices[3].X, hy);
            Point pS = new Point(hx, r.Vertices[0].Y);
            Point pW = new Point(r.Vertices[0].X, hy);
            Point pC = new Point(hx, hy);

            FractalCompression.Structure.Region[] nRegs = new FractalCompression.Structure.Region[4];
            nRegs[0] = new FractalCompression.Structure.Region(r.Vertices[0], new Point(pW.X, pW.Y + 1), new Point(pC.X, pC.Y + 1), pS);
            nRegs[1] = new FractalCompression.Structure.Region(pW, r.Vertices[1], pN, pC);
            nRegs[2] = new FractalCompression.Structure.Region(new Point(pC.X + 1, pC.Y), new Point(pN.X + 1, pN.Y), r.Vertices[2], pE);
            nRegs[3] = new FractalCompression.Structure.Region(new Point(pS.X + 1, pS.Y), new Point(pC.X + 1, pC.Y + 1), new Point(pE.X, pE.Y + 1), r.Vertices[3]);

            for (int i = 0; i < nRegs.Length; i++)
            {
                squeue2.Enqueue(nRegs[i]);
                for (int j = 0; j < 4; j++)
                    AddInterpolationPoint(nRegs[i], j, bitmap);
                /*{
                    Point vert = nRegs[i].Vertices[j];
                    iqueue.Enqueue(new MappedPoint(vert.X, vert.Y, MNTools.GetBitmapValue(vert.X, vert.Y, bitmap)));
                }*/
                
                /*AddInterpolationPoint(nRegs[0], 0, bitmap);
                AddInterpolationPoint(nRegs[0], 1, bitmap);
                AddInterpolationPoint(nRegs[1], 1, bitmap);
                AddInterpolationPoint(nRegs[2], 1, bitmap);
                AddInterpolationPoint(nRegs[2], 2, bitmap);
                for(int j=0; j<4; ++j)
                    AddInterpolationPoint(nRegs[3], j, bitmap);*/
            }
        }

        private void AddInterpolationPoint(FractalCompression.Structure.Region r, int verticeNo, Bitmap bitmap)
        {
            if (verticeNo < 0 || verticeNo > 3)
                throw new Exception("Incorrect vertice number");
            Point vert = r.Vertices[verticeNo];
            iqueue.Enqueue(new MappedPoint(vert.X, vert.Y, MNTools.GetBitmapValue(vert.X, vert.Y, bitmap)));
        }

        public void SaveToFile(String filepath)
        {
            //store dmax, smallDelta, bigDelta, cqueue, iqueue, aqueue
            List<int> aqueueList = null;
            if (this.optimizedAQueue != null)
                aqueueList = optimizedAQueue;
            else
                aqueueList = new List<int>(this.aqueue);

            CompResult results = new CompResult(aqueueList, cqueue, iqueue, bigDelta, smallDelta, a, dmax,bitmap.Width,bitmap.Height);
            StreamWriter sw = new StreamWriter(filepath);
            sw.WriteLine(bigDelta);
            sw.WriteLine(smallDelta);
            sw.WriteLine(a);
            sw.WriteLine(dmax);
            sw.WriteLine(bitmap.Width);
            sw.WriteLine(bitmap.Height);
            sw.WriteLine(aqueueList.Count);
            for (int i = 0; i < aqueueList.Count; i++)
                sw.WriteLine(aqueueList[i]);
            sw.WriteLine(cqueue.Count);
            Queue<Double>.Enumerator enumerator = cqueue.GetEnumerator();
            while (enumerator.MoveNext())
            {
                sw.WriteLine(enumerator.Current);
            }
            sw.WriteLine(iqueue.Count);
            Queue<MappedPoint>.Enumerator enumerator2 = iqueue.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                int x = enumerator2.Current.X;
                sw.WriteLine(x);
                sw.WriteLine(enumerator2.Current.Y);
                sw.WriteLine(enumerator2.Current.Val);
                enumerator2.MoveNext();
                sw.WriteLine(enumerator2.Current.Val);
                enumerator2.MoveNext();
                x = enumerator2.Current.X - x;
                sw.WriteLine(enumerator2.Current.Val);
                enumerator2.MoveNext();
                sw.WriteLine(enumerator2.Current.Val);
                sw.WriteLine(x);
            }
            sw.Close();
        }

        public Queue<MappedPoint> Iqueue
        {
            get { return iqueue; }
        }

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
