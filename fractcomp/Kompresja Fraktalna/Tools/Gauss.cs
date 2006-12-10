using System;
using System.Collections.Generic;
using System.Text;

namespace FractalCompression.Tools
{
    class Gauss
    {
        public static int gauss_cz(double[][] A, double[] b)
        {
            int nr = 0;
            double temp;
            double EPS = 0.000001;
            for (int i = 0; i < A.Length; i++)
            {
                nr = szukaj_czesc(A, i);
                if (nr != i)
                {
                    for (int j = 0; j < A[i].Length; j++)
                    {
                        temp = A[i][j];
                        A[i][j] = A[nr][j];
                        A[nr][j] = temp;
                    }
                    temp = b[i];
                    b[i] = b[nr];
                    b[nr] = temp;
                }
                if (Math.Abs(A[i][i]) < EPS)
                {
                    //macierz osobliwa
                    return -1;
                }

                for (nr = i + 1; nr < A.Length; nr++)
                {
                    temp = A[nr][i];
                    for (int j = i; j < A[i].Length; j++)
                        A[nr][j] -= (A[i][j] * temp) / A[i][i];

                    b[nr] -= b[i] * temp / A[i][i];
                }
            }
            return 0;
        }

        private static int szukaj_czesc(double[][] A, int k)
        {
            int max_nr = k;
            double max_val = Math.Abs(A[k][k]);

            for (int i = k + 1; i < A.Length; i++)
                if (Math.Abs(A[i][k]) > max_val)
                {
                    max_nr = i;
                    max_val = Math.Abs(A[i][k]);
                }
            return max_nr;
        }
    }
}
