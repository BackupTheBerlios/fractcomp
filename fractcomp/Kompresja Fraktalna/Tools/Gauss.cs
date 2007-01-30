using System;
using System.Collections.Generic;
using System.Text;

namespace FractalCompression.Tools
{
    class Gauss
    {
        public static bool GaussianElimination(double[,] a, double[] r)
        {
            double t, s;
            int i, l, j, k, m, n;

            try
            {
                n = r.Length - 1;
                m = n + 1;
                for (l = 0; l <= n - 1; l++)
                {
                    j = l;
                    for (k = l + 1; k <= n; k++)
                    {
                        if (!(Math.Abs(a[j, l]) >= Math.Abs(a[k, l]))) j = k;
                    }
                    if (!(j == l))
                    {
                        for (i = 0; i <= m; i++)
                        {
                            t = a[l, i];
                            a[l, i] = a[j, i];
                            a[j, i] = t;
                        }
                    }
                    for (j = l + 1; j <= n; j++)
                    {
                        t = (a[j, l] / a[l, l]);
                        for (i = 0; i <= m; i++) a[j, i] -= t * a[l, i];
                    }
                }
                r[n] = a[n, m] / a[n, n];
                for (i = 0; i <= n - 1; i++)
                {
                    j = n - i - 1;
                    s = 0;
                    for (l = 0; l <= i; l++)
                    {
                        k = j + l + 1;
                        s += a[j, k] * r[k];
                    }
                    r[j] = ((a[j, m] - s) / a[j, j]);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
