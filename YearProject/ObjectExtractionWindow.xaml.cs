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

        public FeatureExtractionWindow(IEnumerable<String> Filenames,MainWindow caller)
        {
            InitializeComponent();
            this.Filenames.AddRange(Filenames);
            DisplayArea.Text += "\r\nExtracting objects:";

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
                DisplayArea.Text += "\r\nIdentifying Unique Objects:";
                VectorOfVectorOfPoint objects = joinObjects(fileObjects);
                uniqueIdentifierThread.RunWorkerAsync(objects);
                /*
                String dialogText = "Task Complete";
                MessageBox.Show(dialogText, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                */
            }
            Close();
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
            
        }

        private void UniqueIdentifier_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void UniqueIdentifier_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
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
            String dialogText = "Are you sure you want to cancel the current feature extraction?";
            MessageBoxResult res = MessageBox.Show(dialogText, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if cancel selected stop processing and close window
            if (res == MessageBoxResult.Yes)
            {
                objectExtractionThread.CancelAsync();
            }
            
        }
    }
}
