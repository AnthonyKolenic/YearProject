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
        private BackgroundWorker extractionThread;
        private List<String> Filenames = new List<string>();

        public FeatureExtractionWindow(IEnumerable<String> Filenames)
        {
            InitializeComponent();
            this.Filenames.AddRange(Filenames);
            DisplayArea.Text += "\r\n";
            extractionThread = new BackgroundWorker();
            extractionThread.WorkerReportsProgress = true;
            extractionThread.DoWork += new DoWorkEventHandler(Extractor_DoWork);
            extractionThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Extractor_RunWorkerCompleted);
            extractionThread.ProgressChanged += new ProgressChangedEventHandler(Extractor_ProgressChanged);

            extractionThread.RunWorkerAsync(Filenames);

        }

        private void Extractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                String dialogText = "Task Complete";
                MessageBox.Show(dialogText, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Close();
        }

        private void Extractor_DoWork(object sender, DoWorkEventArgs e)
        {
            List<String> filenames = new List<String>((IEnumerable<String>)e.Argument);
            int minHessian = 300;
            BackgroundWorker worker = sender as BackgroundWorker;
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
                    SURF detector = new SURF(minHessian);
                    VectorOfKeyPoint keypoints = new VectorOfKeyPoint();
                    detector.Detect(img,keypoints);
                    //update progress showing another file has been successfully processed
                    worker.ReportProgress(i);
                    
                }
            }
            e.Result = "";
        }

        private void Extractor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progress = (int)(((e.ProgressPercentage + 1) / (double)Filenames.Count<String>()) * 100);
            this.ExtractionProgress.Value = progress;
            this.DisplayArea.Text += Filenames[e.ProgressPercentage] + "\r\n";
            //DisplayArea.Text += filename + "\n";
        }

        private void CancelExtraction_Click(object sender, RoutedEventArgs e)
        {
            extractionThread.CancelAsync();
        }
    }
}
