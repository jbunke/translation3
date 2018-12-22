using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    class MathExt
    {
        /* Returns the Euclidean distance between x and y in pixel units */
        public static double Distance(Point x, Point y)
        {
            return Math.Sqrt(Math.Pow(x.X - y.X, 2) + Math.Pow(x.Y - y.Y, 2));
        }

        /* Returns n if within bounds; min if below; and max if above */
        public static int Bounded(int min, int n, int max)
        {
            Debug.Assert(max >= min);
            return Math.Min(Math.Max(min, n), max);
        }
    }
}
