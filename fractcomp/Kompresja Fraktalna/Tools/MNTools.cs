using System;
using System.Collections.Generic;
using System.Text;
using FractalCompression.Structure;
using System.Drawing;

namespace FractalCompression.Tools
{
    class MNTools
    {
        public static double ComputeContractivityFactor(Domain domain, 
            FractalCompression.Structure.Region region, Bitmap bitmap)
        {
            double u = 0, v = 0;
          /*  double cosu = 0, cosv = 0;
            int ile = 0;
            if (domain.WasUCounted)
                cosu = domain.U;
            else
            {
                for (int i = domain.Vertices[1].X; i < domain.Vertices[2].X; i++)
                    for (int j = domain.Vertices[1].Y; j < domain.Vertices[0].Y; j++)
                    {
                        cosu += bitmap.GetPixel(i, j).B;
                        ile++;
                    }
                cosu = cosu / ile;
                domain.U = cosu;
                domain.WasUCounted = true;
            }
            if (region.WasVCounted)
                cosv = region.V;
            else
            {
                ile = 0;
                for (int i = region.Vertices[1].X; i < region.Vertices[2].X; i++)
                    for (int j = region.Vertices[1].Y; j < region.Vertices[0].Y; j++)
                    {
                        cosv += bitmap.GetPixel(i, j).B;
                        ile++;
                    }
                cosv = cosv / ile;
                region.V = cosv;
                region.WasVCounted = true;
            }
            return cosv / cosu;*/
            //poziomo dla domeny 
            if (domain.WasUCounted)
                u = domain.U;
            else
            {
                u += ComputeVertically2(domain.Vertices[1].X, domain.Vertices[1].Y,
                    domain.Vertices[3].X, domain.Vertices[3].Y, bitmap);
                //pionowa dla domeny
                u += ComputeHorizontally2(domain.Vertices[1].X, domain.Vertices[1].Y,
                    domain.Vertices[3].X, domain.Vertices[3].Y, bitmap);
                u = u / (2 * domain.Size);
                domain.U = u;
                domain.WasUCounted = true;
            }
            if (region.WasVCounted)
                v = region.V;
            else
            {
                //poziomo dla regionu 
                v += ComputeVertically(region.Vertices[1].X, region.Vertices[1].Y,
                    region.Vertices[3].X, region.Vertices[3].Y, bitmap);
                //pionowa dla domeny
                v += ComputeHorizontally(region.Vertices[1].X, region.Vertices[1].Y,
                    region.Vertices[3].X, region.Vertices[3].Y, bitmap);
                v = v / (2 * region.Size);
                region.V = v;
                region.WasVCounted = true;
            }
            double alfa = Math.Log(u / v, Math.E);
            //return 1 / Math.Pow(2, alfa);
            return  v / (3* u);
        }

        private static double ComputeVertically(int x1, int y1, int x2, int y2, Bitmap bitmap)
        {
            double u = 0;
            for (int y = y1; y <= y2; y++)
            {
                double meanVal = 0;
                int yF1 = GetBitmapValue(x1, y, bitmap);
                int yF2 = GetBitmapValue(x2, y, bitmap);
                double a = (yF1 - yF2) / (double)(x1 - x2);
                double b = yF1 - a * x1;
                for (int x = x1; x <= x2; x++)
                {
                    double funVal = a * x + b;
                    int bitmapVal = GetBitmapValue(x, y, bitmap);
                    meanVal += bitmapVal - funVal;
                }
                u += Math.Abs(meanVal) / (x2 - x1);
                //u += meanVal / (x2 - x1);
            }
            return u;
        }

        private static double ComputeVertically2(int x1, int y1, int x2, int y2, Bitmap bitmap)
        {
            double u = 0;
            for (int y = y1; y <= y2; y++)
            {
                double meanVal = 0;
                double max = 0;
                double min = 0;
                int yF1 = GetBitmapValue(x1, y, bitmap);
                int yF2 = GetBitmapValue(x2, y, bitmap);
                double a = (yF1 - yF2) / (double)(x1 - x2);
                double b = yF1 - a * x1;
                for (int x = x1; x <= x2; x++)
                {
                    double funVal = a * x + b;
                    int bitmapVal = GetBitmapValue(x, y, bitmap);
                    double temp = bitmapVal - funVal;
                    if (temp > 0 && temp > max)
                        max = temp;
                    else if (temp < 0 && temp < min)
                        min = temp;
                    //meanVal += bitmapVal - funVal;
                }
                u += max - min;
                //u += Math.Abs(meanVal) / (x2 - x1);
                //u += meanVal / (x2 - x1);
            }
            return u;
        }

        private static double ComputeHorizontally2(int x1, int y1, int x2, int y2, Bitmap bitmap)
        {
            double u = 0;
            for (int x = x1; x <= x2; x++)
            {
                double meanVal = 0;
                double max = 0;
                double min = 0;
                int yF1 = GetBitmapValue(x, y1, bitmap);
                int yF2 = GetBitmapValue(x, y2, bitmap);
                double a = (yF1 - yF2) / (double)(y1 - y2);
                double b = yF1 - a * y1;
                for (int y = y1; y <= y2; y++)
                {
                    double funVal = a * y + b;
                    int bitmapVal = GetBitmapValue(x, y, bitmap);
                    double temp = bitmapVal - funVal;
                    if (temp > 0 && temp > max)
                        max = temp;
                    else if (temp < 0 && temp < min)
                        min = temp;
                    //meanVal += bitmapVal - funVal;
                }
                u += max - min;
                //u += Math.Abs(meanVal) / (x2 - x1);
                // u += meanVal / (x2 - x1);
            }
            return u;
        }

        public static Bitmap RescaleBitmap(Image temp)
        {
            int big = Properties.Settings.Default.bigDelta;
            int modulo = temp.Size.Width % big;
            int times = temp.Size.Width / big;
            int xNewSize = (times + ((modulo >= big / 2) ? 1 : 0)) * big;
            modulo = temp.Size.Height % big;
            times = temp.Size.Height / big;
            int yNewSize = (times + ((modulo >= big / 2) ? 1 : 0)) * big;
            return new Bitmap(temp, xNewSize, yNewSize);
        }

        private static double ComputeHorizontally(int x1, int y1, int x2, int y2, Bitmap bitmap)
        {
            double u = 0;
            for (int x = x1; x <= x2; x++)
            {
                double meanVal = 0;
                int yF1 = GetBitmapValue(x, y1, bitmap);
                int yF2 = GetBitmapValue(x, y2, bitmap);
                double a = (yF1 - yF2) / (double)(y1 - y2);
                double b = yF1 - a * y1;
                for (int y = y1; y <= y2; y++)
                {
                    double funVal = a * y + b;
                    int bitmapVal = GetBitmapValue(x, y, bitmap);
                    meanVal += bitmapVal - funVal;
                }
                u += Math.Abs(meanVal) / (x2 - x1);
               // u += meanVal / (x2 - x1);
            }
            return u;
        }

        public static byte GetBitmapValue(int x, int y, Bitmap bitmap)
        {
            if (x < 0 || y < 0 || x > bitmap.Width || y > bitmap.Height)
                throw new System.IndexOutOfRangeException();
            return bitmap.GetPixel(x, y).B;
        }


        public static double ComputeDistance(FractalCompression.Structure.Region mappedRegion, 
            FractalCompression.Structure.Region region, Bitmap bitmap)
        {
            int h = 0;
            for (int x = mappedRegion.Vertices[0].X; x < mappedRegion.Vertices[3].X; x++)
            {
                for (int y = mappedRegion.Vertices[1].Y; y < mappedRegion.Vertices[0].Y; y++)
                {
                    h += DistanceMeasure(mappedRegion[x % mappedRegion.Size, y % mappedRegion.Size],
                        GetBitmapValue(x, y, bitmap));
                }
            }
            if (h < 0)
                return Int32.MaxValue ;
            return h / (double)(region.Size * region.Size);
        }

        private static int DistanceMeasure(double  val1, double val2)
        {
            return (int)Math.Abs(val1 - val2);
        }
    }
}
