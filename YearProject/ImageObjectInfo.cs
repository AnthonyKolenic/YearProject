﻿using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearProject
{
    public class ImageObjectInfo
    {
       

        public VectorOfKeyPoint KeyPoints { get; set; }
        public LineSegment2D[] Lines { get; set; }
        public VectorOfPoint Contours { get; set; }
    }
}
