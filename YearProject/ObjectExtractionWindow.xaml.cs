using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YearProject
{
    /// <summary>
    /// Interaction logic for FeatureExtractionWindow.xaml
    /// </summary>
    public partial class FeatureExtractionWindow : Window
    {
        private BackgroundWorker objectExtractionThread;
        private BackgroundWorker uniqueIdentifierThread;
        private List<String> Filenames = new List<string>();
        private MainWindow mainReference;
        private Boolean running = false;

        public FeatureExtractionWindow(IEnumerable<String> Filenames,MainWindow caller)
        {
            InitializeComponent();
            this.Filenames.AddRange(Filenames);
            DisplayArea.Text += "\r\nExtracting objects:\r\n";

            objectExtractionThread = new BackgroundWorker();
            objectExtractionThread.WorkerReportsProgress = true;
            objectExtractionThread.WorkerSupportsCancellation = true;
            objectExtractionThread.DoWork += new DoWorkEventHandler(ObjectExtractor_DoWork);
            objectExtractionThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ObjectExtractor_RunWorkerCompleted);
            objectExtractionThread.ProgressChanged += new ProgressChangedEventHandler(ObjectExtractor_ProgressChanged);

            uniqueIdentifierThread = new BackgroundWorker();
            uniqueIdentifierThread.WorkerReportsProgress = true;
            uniqueIdentifierThread.WorkerSupportsCancellation = true;
            uniqueIdentifierThread.DoWork += new DoWorkEventHandler(UniqueIdentifier_DoWork);
            uniqueIdentifierThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UniqueIdentifier_RunWorkerCompleted);
            uniqueIdentifierThread.ProgressChanged += new ProgressChangedEventHandler(UniqueIdentifier_ProgressChanged);

            objectExtractionThread.RunWorkerAsync(Filenames);
            running = true;
            mainReference = caller;
        }

        private void ObjectExtractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);

            }
            else if (e.Cancelled)
            {
            }
            else
            {
                Dictionary<String, VectorOfVectorOfPoint> fileObjects = (Dictionary<String, VectorOfVectorOfPoint>)e.Result;
                mainReference.UpdateFeatureExtractList(fileObjects);
                DisplayArea.Text += "\r\nIdentifying Unique Objects:\r\n";
                VectorOfVectorOfPoint objectContours = joinObjects(fileObjects);
                uniqueIdentifierThread.RunWorkerAsync(objectContours);
                /*
                String dialogText = "Task Complete";
                MessageBox.Show(dialogText, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                */
            }
            
            //Close();
        }

        private void ObjectExtractor_DoWork(object sender, DoWorkEventArgs e)
        {
            List<String> filenames = new List<String>((IEnumerable<String>)e.Argument);
            
            BackgroundWorker worker = sender as BackgroundWorker;
            Dictionary<String, VectorOfVectorOfPoint> result = new Dictionary<string, VectorOfVectorOfPoint>();
            for (int i = 0; i < filenames.Count<String>(); i++)
            {
                String filename = filenames[i];
                
                //if cancel button pressed, cancel task
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //Load Image to process
                    Mat img = CvInvoke.Imread(filename, Emgu.CV.CvEnum.ImreadModes.AnyColor);
                    if (img.IsEmpty) // if failed to read image in
                    {
                        //display error
                        String dialogText = "File not found: \"" + filename + "\", Press OK to ignore or Cancel to stop extracting features";
                        MessageBoxResult res = MessageBox.Show(dialogText, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        //if cancel selected stop processing and close window
                        if (res == MessageBoxResult.Cancel)
                        {
                            e.Cancel = true;
                            break;
                        }
                        //if user chose to ignore, jump over filename and attempt to process the other files
                        continue;
                        
                    }
                    //-----------------process file-----------------------
                    
                    result.Add(filename, ImageManipulation.extractContours(img));
                    worker.ReportProgress(i);
                    
                }
            }
            e.Result = result;
        }

       

        private void ObjectExtractor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progress = (int)(((e.ProgressPercentage + 1) / (double)Filenames.Count<String>()) * 100);
            this.ExtractionProgress.Value = progress;
            this.DisplayArea.Text += Filenames[e.ProgressPercentage] + "\r\n";
        }

        private void UniqueIdentifier_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CancelExtraction.Content = "Close";
            running = false;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);

            }
            else if (e.Cancelled)
            {
            }
            else
            {
                 VectorOfVectorOfPoint contours = (VectorOfVectorOfPoint)e.Result;
                mainReference.setObjects(contours);
                
                /*
                String dialogText = "Task Complete";
                MessageBox.Show(dialogText, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                */
            }
            
        }

        private void UniqueIdentifier_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            VectorOfVectorOfPoint objectContours = (VectorOfVectorOfPoint)e.Argument;
            int size = objectContours.Size;
            int state = 0;
            worker.ReportProgress(state);

            //Generate a boundbox for each countour
            BoundBox[] boundBoxes = new BoundBox[size];
            for (int i = 0; i < size; i++)
            {
                boundBoxes[i] = getBoundBox(objectContours[i]);
            }
            worker.ReportProgress(++state);

            //Create set of offsetted countours for image generation
            VectorOfVectorOfPoint offsetObjects = new VectorOfVectorOfPoint(size);
            for (int i = 0; i < size; i++)
            {
                offsetObjects.Push(offsetContour(objectContours[i],boundBoxes[i].TopLeft));
            }
            worker.ReportProgress(++state);

            //create images of each of the contours
            Image<Bgr, Byte>[] images = new Image<Bgr, byte>[size];
            for (int i = 0; i < size;i++)
            {
                BoundBox box = boundBoxes[i];
                images[i] = new Image<Bgr, byte>(box.Width,box.Height,new Bgr(255,255,255));
                CvInvoke.DrawContours(images[i], offsetObjects[i], -1, new MCvScalar(0, 0, 0));
            }
            worker.ReportProgress(++state);
            e.Result = objectContours;
            //TODO (Anthony): Extract keypoints and compare rest of images
        }

        private void UniqueIdentifier_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                {
                    this.DisplayArea.Text += "Creating boundboxes\r\n";
                    break;
                }
                case 1:
                {
                    this.DisplayArea.Text += "Offsetting objects\r\n";
                    break;
                }
                case 2:
                {
                    this.DisplayArea.Text += "Drawing objects for feature extraction\r\n";
                    break;
                }
                case 3:
                {
                    this.DisplayArea.Text += "Extracting features from drawn objects\r\n";
                    break;
                }
                default:
                {
                    this.DisplayArea.Text += "Unknown State " + e.ProgressPercentage + "\r\n";
                    break;
                }
            }
        }

        private VectorOfVectorOfPoint joinObjects(Dictionary<String, VectorOfVectorOfPoint> objects)
        {
            VectorOfVectorOfPoint res = new VectorOfVectorOfPoint();
            //loop over files
            foreach (String currentFile in objects.Keys)
            {
                //loop over objects in image file
                for (int i = 0; i < objects[currentFile].Size;i++)
                {
                    res.Push(objects[currentFile][i]);
                }
            }
            return res;
        }


        private void CancelExtraction_Click(object sender, RoutedEventArgs e)
        {
            if (running)
            {
                String dialogText = "Are you sure you want to cancel the current feature extraction?";
                MessageBoxResult res = MessageBox.Show(dialogText, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                //if cancel selected stop processing and close window
                if (res == MessageBoxResult.Yes)
                {
                    objectExtractionThread.CancelAsync();
                }
            }
            else
            {
                Close();
            }
        }

        /*
         *  Creates bound box that contains all the points 
         */
        private BoundBox getBoundBox(VectorOfPoint points)
        {
            int smallestX = points[0].X;
            int smallestY = points[0].Y;
            int largestX = points[0].X;
            int largestY = points[0].Y;
            for (int i = 1; i < points.Size;i++)
            {
                if (points[i].X < smallestX) smallestX = points[i].X;
                if (points[i].Y < smallestY) smallestY = points[i].Y;
                if (points[i].X > largestX) largestX = points[i].X;
                if (points[i].Y > largestY) largestY = points[i].Y;
            }
            BoundBox res = new BoundBox(smallestX,smallestY,largestX,largestY);
            return res;
        }

        /*
         *  Offset contour by a point
         */
        private VectorOfPoint offsetContour(VectorOfPoint contour, System.Drawing.Point offset)
        {
            System.Drawing.Point[] points = new System.Drawing.Point[contour.Size];
            for (int i = 0; i < contour.Size;i++)
            {
                points[i] = new System.Drawing.Point(contour[i].X - offset.X, contour[i].Y - offset.Y);
            }
            return new VectorOfPoint(points);
        }
    }
}
