using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearProject
{
    abstract class Shape
    {
        private String _ID;
        private Point _point;

        public String ID
        {
            get
            {
                return _ID;
            }
        }

        public Point Point{get; set;}

    }
}
