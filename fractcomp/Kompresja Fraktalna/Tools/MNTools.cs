using System;
using System.Collections.Generic;
using System.Text;
using FractalCompression.Structure;
using System.Drawing;

namespace FractalCompression.Tools
{
    class MNTools
    {
        public static double ComputeContractivityFactor(Domain domain, FractalCompression.Structure.Region region,
            Bitmap bitmap)
        {
            double u = 0, v = 0;
            //poziomo dla domeny 
            u += ComputeVertically(domain.Vertices[0].X, domain.Vertices[0].Y,
                domain.Vertices[3].X, domain.Vertices[1].Y, bitmap);
            //pionowa dla domeny
            u += ComputeHorizontally(domain.Vertices[0].X, domain.Vertices[0].Y,
                domain.Vertices[3].X, domain.Vertices[1].Y, bitmap);
            u = u / (2 * domain.Size);
            //poziomo dla regionu 
            v += ComputeVertically(region.Vertices[0].X, region.Vertices[0].Y,
                region.Vertices[3].X, region.Vertices[1].Y, bitmap);
            //pionowa dla domeny
            v += ComputeHorizontally(region.Vertices[0].X, region.Vertices[0].Y,
                region.Vertices[3].X, region.Vertices[1].Y, bitmap);
            v = v / (2 * region.Size);
            return  v / u;
        }

        private static double ComputeVertically(int x1, int y1, int x2, int y2, Bitmap bitmap)
        {
            double u = 0;
            for (int y = y1; y <= y2; y++)
            {
                double meanVal = 0;
                int yF1 = bitmap.GetPixel(x1, y).B;
                int yF2 = bitmap.GetPixel(x2, y).B;
                double a = (yF1 - yF2) / (double)(x1 - x2);
                double b = yF1 - a * x1;
                for (int x = x1; x <= x2; x++)
                {
                    double funVal = a * x + b;
                    int bitmapVal = bitmap.GetPixel(x, y).B;
                    meanVal += bitmapVal - funVal;
                }
                u += Math.Abs(meanVal) / (x2 - x1);
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
                int yF1 = bitmap.GetPixel(x, y1).B;
                int yF2 = bitmap.GetPixel(x, y2).B;
                double a = (yF1 - yF2) / (double)(y1 - y2);
                double b = yF1 - a * y1;
                for (int y = y1; y <= y2; y++)
                {
                    double funVal = a * y + b;
                    int bitmapVal = bitmap.GetPixel(x, y).B;
                    meanVal += bitmapVal - funVal;
                }
                u += Math.Abs(meanVal) / (x2 - x1);
            }
            return u;
        }

        public static byte GetBitmapValue(int x, int y, Bitmap bitmap)
        {
            if (x < 0 || y < 0 || x > bitmap.Width || y > bitmap.Height)
                throw new Exception("Index out of bound");
            return bitmap.GetPixel(x, y).B;
        }


        public static double ComputeDistance(FractalCompression.Structure.Region mappedRegion, 
            FractalCompression.Structure.Region region, Bitmap bitmap)
        {
            double h = 0;
            for (int x = mappedRegion.Vertices[0].X; x <= mappedRegion.Vertices[3].X; x++)
            {
                for (int y = mappedRegion.Vertices[0].Y; y <= mappedRegion.Vertices[1].Y; y++)
                {
                    h += DistanceMeasure(mappedRegion[x - mappedRegion.Size, y - mappedRegion.Size],
                        bitmap.GetPixel(x, y).B);
                }
            }
            return h;
        }

        private static double DistanceMeasure(double  val1, double val2)
        {
            return val1 - val2;
        }
    }
}
