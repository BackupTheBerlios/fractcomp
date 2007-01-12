using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FractalCompression.Structure;

namespace FractalCompression.Tools
{
    class Decompressor
    {
        List<double> contractivityFactors;
        List<SimpledRegion> regions;
        //nie wiem co zawiera ta lista, wiec jak bedziesz wiedzial to bede
        //wdzieczny za zmiane
        List<int> addresses;
        int smallDelta, bigDelta;
        int a;
        int width;
        int height;
        int dMax;

        public Decompressor(List<double> contrctivtyFactors,
            List<SimpledRegion> regions, List<int> addresses,
            int smallDelta, int bigDelta, int a, int width, int height, int dMax)
        {
            this.contractivityFactors = contrctivtyFactors;
            this.regions = regions;
            this.addresses = addresses;
            this.a = a;
            this.smallDelta = smallDelta;
            this.bigDelta = bigDelta;
            this.width = width;
            this.height = height;
            this.dMax = dMax;
        }

        public Bitmap DecompressImage()
        {
            int steps = (int)Math.Truncate(Math.Log(smallDelta, 2) / Math.Log(a, 2));
            Bitmap bit = new Bitmap(width, height, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            foreach (SimpledRegion sr in regions)
            {
                for (int i = 0; i < sr.Vertices.Length; i++)
                    bit.SetPixel(sr.Vertices[i].X, sr.Vertices[i].Y,
                        Color.FromArgb(sr.Values[i],
                        sr.Values[i],
                        sr.Values[i]));
            }
            for (int t = 0; t < steps; t++)
            {
                for (int i = 0; i < Math.Min(steps, dMax); i++)
                {
                    for (int j = 0; j < regions.Count; j++)
                    {
                        int coresspondingDomain = addresses[j];
                        
                        if (j != -1)
                        {
                            double contractivityFactor = contractivityFactors[j];
                            for (int h = 0; h < 1 ; h++)
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
