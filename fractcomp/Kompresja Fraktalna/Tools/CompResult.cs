using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace FractalCompression.Tools
{
    [Serializable]
    class CompResult
    {
        private int bigDelta, smallDelta, dmax, a;
        private int imageWidth, imageHeight;

        private Queue<MappedPoint> iqueue;
        private Queue<double> cqueue;                   //contractivity factors
        private List<int> aqueue;                      //addresses domain's

        public CompResult(List<int> aqueue, Queue<double> cqueue, Queue<MappedPoint> iqueue, int bigDelta, int smallDelta, int a, int dmax, int imageWidth, int imageHeight) 
        {
            this.aqueue = aqueue;
            this.cqueue = cqueue;
            this.iqueue = iqueue;

            this.bigDelta = bigDelta;
            this.smallDelta = smallDelta;
            this.dmax = dmax;
            this.a = a;

            this.imageHeight = imageHeight;
            this.imageWidth = imageWidth;
        }

        public Queue<MappedPoint> Iqueue
        {
            get { return iqueue; }
        }

        public int DMax
        {
            get { return dmax; }
        }

        public List<int> Aqueue
        {
            get { return aqueue; }
        }

        public Queue<double> Cqueue
        {
            get { return cqueue; }
        }

        public int BigDelta
        {
            get { return bigDelta; }
        }

        public int SmallDelta
        {
            get { return smallDelta; }
        }

        public int ImageWidth
        {
            get { return imageWidth; }
        }

        public int ImageHeight
        {
            get { return imageHeight; }
        }

        public int A
        {
            get { return a; }
        }
    }
}
