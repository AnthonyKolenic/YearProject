using Emgu.CV.Util;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YearProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageListWindow imageList ;
        List<String> inputImageFilenames = new List<string>();
        bool featuresExtractValid = true;
        Dictionary<String, VectorOfKeyPoint> keypoints;

        public MainWindow()
        {
            InitializeComponent();
            imageList = new ImageListWindow(this);
            imageList.Show();
        }

        public void UpdateImageList(IEnumerable<String> items)
        {
            inputImageFilenames.Clear();
            featuresExtractValid = false;
            inputImageFilenames.AddRange(items);
        }

        public void UodateFeatureExtractList(Dictionary<String, VectorOfKeyPoint> keypoints)
        {
            featuresExtractValid = true;
            this.keypoints = keypoints;
            System.Console.WriteLine("Wooh");
        }

        private void ImageList_Clicked(object sender, RoutedEventArgs e)
        {
            imageList.Show();
        }

        private void ExtractFeatures_Clicked(object sender, RoutedEventArgs e)
        {
            int numFiles = inputImageFilenames.Count;
            if (numFiles > 0)
            {
                bool execute = true;
                if (featuresExtractValid)
                {
                    if (MessageBox.Show("Keypoints are still valid, are you sure you want to update them?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        execute = false;

                    }
                }
                if (execute)
                {
                    String dialogText = "Are you sure you want to extract features from " + numFiles + " image(s), this could take a long time?";
                    MessageBoxResult res = MessageBox.Show(dialogText, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        FeatureExtractionWindow extractWindow = new FeatureExtractionWindow(inputImageFilenames, this);
                        extractWindow.ShowDialog();

                    }
                }
            } 
            else
            {
                MessageBox.Show("No files selected for processing, add files using File->Image List", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            imageList.ForceClose();

        }
    }
}
