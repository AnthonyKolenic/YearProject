using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearProject
{
    class ImageManipulation
    {
        /**
         * Extracts all the important points from a Mat image 
         **/
        public static VectorOfKeyPoint extractKeyPoints(Mat image, int minHessian = 400)
        {
            VectorOfKeyPoint res = new VectorOfKeyPoint();
            SURF detector = new SURF(minHessian);
            detector.DetectRaw(image, res);
            return res;
        }

        /**
         * Extracts all contours of all objects within a image
         **/
        public static VectorOfVectorOfPoint extractContours(Mat image)
        {
            VectorOfVectorOfPoint res = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(getEdges(image), res, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            return res;
        }

        /**
         * Extracts lines from image
         **/
        public static LineSegment2D[] extractLines(Mat image)
        {
            LineSegment2D[] res = null;
            res = CvInvoke.HoughLinesP(getEdges(image), 1, Math.PI / 45.0, 20, 30, 10);
            return res;
        }

        /*
         *  Creates bound box that contains all the points 
         */
        public static BoundBox getBoundBox(VectorOfPoint points)
        {
            int smallestX = points[0].X;
            int smallestY = points[0].Y;
            int largestX = points[0].X;
            int largestY = points[0].Y;
            for (int i = 1; i < points.Size; i++)
            {
                if (points[i].X < smallestX) smallestX = points[i].X;
                if (points[i].Y < smallestY) smallestY = points[i].Y;
                if (points[i].X > largestX) largestX = points[i].X;
                if (points[i].Y > largestY) largestY = points[i].Y;
            }
            BoundBox res = new BoundBox(smallestX, smallestY, largestX, largestY);
            return res;
        }

        /*
         *  Offset contour by a point
         */
        public static VectorOfPoint offsetContour(VectorOfPoint contour, System.Drawing.Point offset)
        {
            System.Drawing.Point[] points = new System.Drawing.Point[contour.Size];
            for (int i = 0; i < contour.Size; i++)
            {
                points[i] = new System.Drawing.Point(contour[i].X - offset.X, contour[i].Y - offset.Y);
            }
            return new VectorOfPoint(points);
        }

        private static UMat getScrubbedImage(Mat image)
        {
            UMat res = new UMat();
            //convert image to grey
            CvInvoke.CvtColor(image, res, ColorConversion.Bgr2Gray);

            //Get rid of noise by going pyramid up and down
            {
                UMat tmp = new UMat();
                CvInvoke.PyrDown(res, tmp);
                CvInvoke.PyrUp(tmp, res);
            }
            return res;
        }
        
        private static UMat getEdges(Mat image, double linkThres = 120.0, double cannyThres = 180.0)
        {
            UMat res = new UMat();
            CvInvoke.Canny(getScrubbedImage(image), res, cannyThres, linkThres);
            return res;
        }

        

    }
}
