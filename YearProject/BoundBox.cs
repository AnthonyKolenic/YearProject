using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace YearProject
{
    class BoundBox
    {
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }

        public BoundBox()
        {

        }

        public BoundBox(int topX,int topY,int bottomX,int bottomY)
        {
            TopLeft = new Point(Math.Min(topX,bottomX), Math.Min(topY, bottomY));
            BottomRight = new Point(Math.Max(topX, bottomX), Math.Max(topY, bottomY));
        }

        public int Width
        {
            get
            {
                return (int)Math.Round((double)BottomRight.X - TopLeft.X);
            }
        }

        public int Height
        {
            get
            {
                return (int)Math.Round((double)BottomRight.Y - TopLeft.Y);
            }
        }
    }
}
