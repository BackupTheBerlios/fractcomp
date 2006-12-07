using System;
using System.Collections.Generic;
using System.Text;
using FractalCompression.Structure;
using System.Drawing;

namespace FractalCompression.Tools
{
    class MNTools
    {
        public double ComputeContractivityFactor(Domain domain, FractalCompression.Structure.Region region,
            Bitmap bitmap)
        {
            double u = 0;
            for (int y = domain.Vertices[0].Y; y <= domain.Vertices[1].Y; y++)
            {
                double meanVal = 0;
                for (int x = domain.Vertices[0].X; x <= domain.Vertices[3].X; x++)
                {
                    int bitmapVal = bitmap.GetPixel(x, y).B;
                    meanVal += bitmapVal;
                }
                u += meanVal / domain.Size;
            }
            u = u / domain.Size;
            return 1;
        }

        public double ComputeDistance(FractalCompression.Structure.Region mappedRegion, 
            FractalCompression.Structure.Region region)
        {
            return 1;
        }
    }
}
