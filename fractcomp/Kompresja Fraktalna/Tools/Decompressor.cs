using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FractalCompression.Tools
{
    class Decompressor
    {
        List<double> contrctivtyFactors;
        List<MappedPoint> interpolationPoints;
        //nie wiem co zawiera ta lista, wiec jak bedziesz wiedzial to bede
        //wdzieczny za zmiane
        List<int> addresses;
        //wielkosci kolejnych regionow, najczesciej bede te same, ale
        //jak dojdzie koniecznosc podzialu regionu na podregiony to musze o tym wiedziec
        List<int> regionSizes;
        int smallDelta;
        int a;
        int width;
        int height;
        int dmax = 10;

        public Decompressor(List<double> contrctivtyFactors,
            List<MappedPoint> interpolationPoints, List<int> addresses,
            List<int> regionSizes,int smallDelta, int a, int width, int height)
        {
            this.contrctivtyFactors = contrctivtyFactors;
            this.interpolationPoints = interpolationPoints;
            this.addresses = addresses;
            this.a = a;
            this.smallDelta = smallDelta;
            this.width = width;
            this.height = height;
            this.regionSizes = regionSizes;
        }

        public Bitmap DecompressImage()
        {
            int steps = (int)Math.Truncate(Math.Log(smallDelta, 2) / Math.Log(a, 2));
            Bitmap bit = new Bitmap(width, height, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            foreach (MappedPoint mp in interpolationPoints)
                bit.SetPixel(mp.X, mp.Y, Color.FromArgb((int)mp.Val, (int)mp.Val, (int)mp.Val));
            for (int t = 0; t < steps; t++)
            {
                for (int i = 0; i < Math.Min(steps, dmax); i++)
                {
                    for (int j = 0; j < interpolationPoints.Count; j++)
                    {
                        int coresspondingDomain = addresses[j];
                        if (j != 0)
                        {
                            double contractivityFactor = contrctivtyFactors[j];
                            for (int h = 0; h < regionSizes[j]; h++)
                            {

                            }
                        }
                    }
                }
            }
            return bit;
        }
    }
}
