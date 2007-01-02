using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace FractalCompression.Tools
{
    [Serializable]
    class CompResult
    {
        private int bigDelta, smallDelta, dmax;

        private Queue<Point3D> iqueue;
        private Queue<double> cqueue;               //contractivity factors
        private Queue<int> aqueue;                     //addresses domain's

        public CompResult(Queue<int> aqueue, Queue<double> cqueue, Queue<Point3D> iqueue, int bigDelta, int smallDelta, int dmax)
        {
            this.aqueue = aqueue;
            this.cqueue = cqueue;
            this.iqueue = iqueue;

            this.bigDelta = bigDelta;
            this.smallDelta = smallDelta;
            this.dmax = dmax;
        }

        public Queue<Point3D> Iqueue
        {
            get { return iqueue; }
        }

        public int DMax
        {
            get { return dmax; }
        }

        public Queue<int> Aqueue
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
    }
}
