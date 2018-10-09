using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearProject
{
    class Detector
    {
        public Point Location { get; set; }
        public int Radius { get; set; }
        
        public Detector()
        {

        }

        public Detector(int x,int y, int r)
        {
            Location = new Point(x, y);
            Radius = r;
        }
    }
}
