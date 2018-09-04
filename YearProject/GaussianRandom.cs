using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearProject
{
    class GaussianRandom
    {
        Random random;

        public GaussianRandom()
        {
            random = new Random();
        }

        public GaussianRandom(int seed)
        {
            random = new Random(seed);
        }

        public double NextValue()
        {
            //box muller transform, check wolfram for formula
            double xA = random.NextDouble();
            double xB = random.NextDouble();
            return Math.Sqrt(-2 * Math.Log(xA, Math.E)) * Math.Cos(2 * Math.PI * xB);
        }
    }
}
