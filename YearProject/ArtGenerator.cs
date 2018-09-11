using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearProject
{
    class ArtGenerator
    {
        Settings settings;
        Random randomGen;
        GaussianRandom gausRandomGen;
        Emgu.CV.Util.VectorOfVectorOfPoint objects;

        public ArtGenerator(Settings settings, VectorOfVectorOfPoint objects)
        {
            this.settings = settings;
            this.objects = objects;
            randomGen = new Random(settings.Seed);
            gausRandomGen = new GaussianRandom(settings.Seed);
        }
    }
}
