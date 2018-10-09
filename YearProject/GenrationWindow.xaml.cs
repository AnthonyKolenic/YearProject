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
    /// Interaction logic for GenrationWindow.xaml
    /// </summary>
    public partial class GenerationWindow : Window
    {
        struct DataPass
        {
            public Settings settings;
            public Emgu.CV.Util.VectorOfVectorOfPoint objects;
            public Dictionary<String, Emgu.CV.Util.VectorOfVectorOfPoint> inputData;
        }

        private BackgroundWorker generatorThread;
        public GenerationWindow(Settings settings, Emgu.CV.Util.VectorOfVectorOfPoint contours,Dictionary<String, Emgu.CV.Util.VectorOfVectorOfPoint> features)
        {
            InitializeComponent();
            generatorThread = new BackgroundWorker();
            generatorThread.WorkerReportsProgress = true;
            generatorThread.WorkerSupportsCancellation = true;
            generatorThread.DoWork += new DoWorkEventHandler(Generator_DoWork);
            generatorThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Generator_RunWorkerCompleted);
            generatorThread.ProgressChanged += new ProgressChangedEventHandler(Generator_ProgressChanged);
            DataPass tmp = new DataPass();
            tmp.settings = settings;
            tmp.objects = contours;
            tmp.inputData = features;
            generatorThread.RunWorkerAsync(tmp);

        }

        private void CancelExtraction_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Generator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                
            }
        }

        private void Generator_DoWork(object sender, DoWorkEventArgs e)
        {

            DataPass data = (DataPass)e.Argument;
            //e.Result = result;
        }



        private void Generator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

    }
}
