using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using FractalCompression.Structure;

namespace FractalCompression.Tools
{
    class POTools
    {
        public static bool CheckConditionOfContinuity(Domain[,] domains, double[,] s, int a, Bitmap bitmap)
        {
            for (int i = 0; i < domains.GetUpperBound(0)-1; i++)
                for (int j = 0; j < domains.GetUpperBound(1)-1; j++)
                    for (int v = 1; v < a; v++)
                    {
                        if (s[i, j] * domains[i, j].Right(v, bitmap) != s[i + 1, j] * domains[i + 1, j].Left(v, bitmap))
                            return false;
                        if (s[i, j] * domains[i, j].Up(v, bitmap) != s[i, j + 1] * domains[i, j + 1].Down(v, bitmap))
                            return false;
                    }
            return true;
        }

        public static bool CheckConditionOfContinuity(Domain[,] domains, int i, int j, int a, FractalCompression.Structure.Region region, Bitmap bitmap)
        {
            if (i<0 || j<0 || (i > domains.GetUpperBound(0) - 1) || (j > domains.GetUpperBound(1) - 1))
                throw new ArgumentException("Incorrect i or j values");

            Domain dij = domains[i, j];
            Domain dip1j = domains[i + 1, j];
            Domain dijp1 = domains[i, j + 1];

            double sij = MNTools.ComputeContractivityFactor(dij, region, bitmap);
            double sip1j = MNTools.ComputeContractivityFactor(dip1j, region, bitmap);
            double sijp1 = MNTools.ComputeContractivityFactor(dijp1, region, bitmap);

            //TODO: spr czy na pewno v=0 (a nie 1) i v< a-1 a nie (v<a)
            for (int v = 0; v < a-1 ; v++)
            {
                if (sij * dij.Right(v, bitmap) != sip1j * dip1j.Left(v, bitmap))
                    return false;
                if (sij * dij.Up(v, bitmap) != sijp1 * dijp1.Down(v, bitmap))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns parameters A,B i C of line Ax+By+C=0 crossing points p and r
        /// </summary>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static double[] LineBetweenPointsParameters(PointF p, PointF r)
        {
            double a = r.Y - p.Y, b = r.X - p.X;
            double[] ABC ={ a, b, b * p.Y - a * p.X };
            return ABC;
        }

        public static int DistanceSignum(PointF point, PointF lineStart, PointF lineEnd)
        {
            double[] lineABC = LineBetweenPointsParameters(lineStart, lineEnd);
            double lineVal = (lineABC[0] * point.X + lineABC[2]) / -lineABC[1];
            if (lineVal < point.Y)
                return 1;
            else if (lineVal > point.Y)
                return -1;
            else
                return 0;
        }

        public static FractalCompression.Structure.Region MapDomainToRegion(Domain domain, FractalCompression.Structure.Region region, Bitmap bitmap, Mapper mapper, int a)
        {
            int maxX = region.MappedVals.GetUpperBound(0);
            int maxY = region.MappedVals.GetUpperBound(1);
            double[,] mappedVals = new double [maxX,maxY];

            int x, y;
            for (int i = 0; i < maxX; ++i)
            {
                if (i == 0 )
                {
                    mappedVals[i, 0] = MNTools.GetBitmapValue(region.Vertices[1].X, region.Vertices[1].Y,bitmap);
                    mappedVals[i, maxY - 1] = MNTools.GetBitmapValue(region.Vertices[2].X, region.Vertices[2].Y,bitmap);
                    for (int j = 1; j < maxY - 1; ++j)
                    {
                        x = region.Vertices[1].X + i;
                        y = region.Vertices[1].Y + (j + 1) + a-1;
                        mappedVals[i, j] = mapper.MapPoint(x, y, MNTools.GetBitmapValue(x, y,bitmap)).Val;
                    }
                }
                else if  (i == maxX - 1)
                {
                    mappedVals[i, 0] = MNTools.GetBitmapValue(region.Vertices[2].X, region.Vertices[2].Y, bitmap);
                    mappedVals[i, maxY - 1] = MNTools.GetBitmapValue(region.Vertices[3].X, region.Vertices[3].Y, bitmap);
                    for (int j = 1; j < maxY - 1; ++j)
                    {
                        x = region.Vertices[1].X + i;
                        y = region.Vertices[1].Y + (j + 1) + a-1;
                        mappedVals[i, j] = mapper.MapPoint(x, y, MNTools.GetBitmapValue(x, y, bitmap)).Val;
                    }
                }
                else
                    for (int j = 0; j < maxY; ++j)
                    {
                        x = region.Vertices[1].X + i;
                        y = region.Vertices[1].Y  + j + a-1;
                        mappedVals[i, j] = mapper.MapPoint(x, y, MNTools.GetBitmapValue(x, y, bitmap)).Val;
                    }
            }

            FractalCompression.Structure.Region mappedRegion = new FractalCompression.Structure.Region(region.Vertices, mappedVals);
            return mappedRegion;
        }

        // a - regionsInDomainRow 
        /*public static List<MappedPoint> ComputeInterpolationPoints0(Bitmap bitmap, int bigDelta, int a)
        {
            List<MappedPoint> interpolPoints = new List<MappedPoint>();
            int smallDelta = bigDelta / a;

            int regionsInColumn = bitmap.Height / smallDelta;
            int domainsInRow = bitmap.Width / bigDelta;
            int regionsInRow = domainsInRow * a;

            int eastX, northY = 0;
            MappedPoint mp2 = null, mp3 = null;
            for (int i = 0; i < regionsInColumn; ++i)
            {
                eastX = 0;
                mp2 = mp3 = null;
                for (int j = 0; j < regionsInRow; ++j)
                {
                    if (mp2 != null && mp3 != null)
                    {
                        interpolPoints.Add(mp3);
                        interpolPoints.Add(mp2);
                    }
                    else
                    {
                        interpolPoints.Add(CreateMappedPoint(bitmap, eastX, northY + smallDelta));
                        interpolPoints.Add(CreateMappedPoint(bitmap, eastX, northY));
                    }

                    eastX += smallDelta;
                    mp2 = CreateMappedPoint(bitmap, eastX, northY);
                    mp3 = CreateMappedPoint(bitmap, eastX, northY + smallDelta);
                    interpolPoints.Add(mp2);
                    interpolPoints.Add(mp3);
                }
                northY += smallDelta;
            }

            return interpolPoints;
        }*/

        // a - regionsInDomainRow 
        private static FractalCompression.Structure.Region[,] PrepareRegions(Bitmap bitmap, int bigDelta, int a, out FractalCompression.Structure.Region[,] regions, out List<MappedPoint> interpolPoints)
        {
            int smallDelta = bigDelta / a;
            int regionsInColumn = bitmap.Height / smallDelta;
            int domainsInRow = bitmap.Width / bigDelta;
            int regionsInRow = domainsInRow * a;

            regions = new FractalCompression.Structure.Region[regionsInColumn, regionsInRow];
            interpolPoints = new List<MappedPoint>();

            int eastX, northY = 0;
            MappedPoint[] mp = new MappedPoint[4];
            for (int i = 0; i < regionsInColumn; ++i)
            {
                eastX = 0;
                for (int j = 0; j < regionsInRow; ++j)
                {
                    mp[0] = CreateMappedPoint(bitmap, eastX, northY + smallDelta - 1);
                    mp[1] = CreateMappedPoint(bitmap, eastX, northY);
                    mp[2] = CreateMappedPoint(bitmap, eastX + smallDelta - 1, northY);
                    mp[3] = CreateMappedPoint(bitmap, eastX + smallDelta - 1, northY + smallDelta - 1);
                    for (int k = 0; k < mp.Length; ++k)
                        interpolPoints.Add(mp[k]);

                    regions[i, j] = new FractalCompression.Structure.Region(mp);
                    eastX += smallDelta;
                }
                northY += smallDelta;
            }

            return regions;
        }

        // a - regionsInDomainRow
        private static Domain[,] PrepareDomains(int bigDelta, int a, int bmpWidth, int bmpHeight, FractalCompression.Structure.Region[,] regions)
        {
            int domainsInRow = bmpWidth / bigDelta;
            int domainsInColumn = bmpHeight / bigDelta;
            Domain[,] domains = new Domain[domainsInColumn, domainsInRow];

            Point[] vertices = new Point[4];
            int eastX, northY = 0;
            for (int i = 0; i < domainsInColumn; ++i)
            {
                eastX = 0;
                for (int j = 0; j < domainsInRow; ++j)
                {
                    vertices[0] = new Point(eastX, northY + bigDelta - 1);
                    vertices[1] = new Point(eastX, northY);
                    vertices[2] = new Point(eastX + bigDelta - 1, northY);
                    vertices[3] = new Point(eastX + bigDelta - 1, northY + bigDelta - 1);

                    domains[i, j] = new FractalCompression.Structure.Domain(vertices, a);
                    eastX += bigDelta;
                }
                northY += bigDelta;
            }
            return domains;
        }

        public static void PrepareStructures(Bitmap bitmap, int bigDelta, int a,
            out FractalCompression.Structure.Region[,] regions, out FractalCompression.Structure.Domain[,] domains, out List<MappedPoint> interpolPoints)
        {
            regions = PrepareRegions(bitmap, bigDelta, a, out regions, out interpolPoints);
            domains = PrepareDomains(bigDelta, a, bitmap.Width, bitmap.Height, regions);
        }

        private static MappedPoint CreateMappedPoint(Bitmap bitmap, int x, int y)
        {
            double val;
            if (x == bitmap.Width)
                x = bitmap.Width - 1;
            if (y == bitmap.Height)
                y = bitmap.Height - 1;

            val = MNTools.GetBitmapValue(x, y, bitmap);
            MappedPoint mp = new MappedPoint(x, y, val);
            return mp;
        }

        public static void PrintInterpolationPoints(List<MappedPoint> intrpList)
        {
            Console.WriteLine();
            Console.WriteLine("interpolation points: " + intrpList.Count);
            for (int i = 0; i < intrpList.Count; i++)
            {
                Console.Write("({0},{1}), ", intrpList[i].X, intrpList[i].Y);
                if ((i + 1) % 4 == 0)
                    Console.WriteLine();
            }
        }

        public static void PrintDomains(FractalCompression.Structure.Domain[,] domains)
        {
            Console.WriteLine();
            Console.WriteLine("domains: " + domains.Length);
            for (int i = 0; i <= domains.GetUpperBound(0); ++i)
                for (int j = 0; j <= domains.GetUpperBound(1); ++j)
                {
                    Point[] v = domains[i, j].Vertices;
                    Console.WriteLine("({0}, {1}, {2}, {3})", v[0], v[1], v[2], v[3]);
                }
        }

        public static void PrintRegions(FractalCompression.Structure.Region[,] regions)
        {
            Console.WriteLine();
            Console.WriteLine("regions: " + regions.Length);
            for (int i = 0; i <= regions.GetUpperBound(0); ++i)
                for (int j = 0; j <= regions.GetUpperBound(1); ++j)
                {
                    Point[] v = regions[i, j].Vertices;
                    Console.WriteLine("({0}, {1}, {2}, {3})", v[0], v[1], v[2], v[3]);
                }
        }

        public static CompResult DeserializeCompResult(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            CompResult result = (CompResult)bf.Deserialize(fs);
            fs.Close();
            return result;
        }
    }
}
