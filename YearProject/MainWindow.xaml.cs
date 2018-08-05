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

        public MainWindow()
        {
            InitializeComponent();
            imageList = new ImageListWindow(this);
            imageList.Show();
        }

        public void UpdateImageList(IEnumerable<String> items)
        {
            inputImageFilenames.Clear();
            inputImageFilenames.AddRange(items);
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
                String dialogText = "Are you sure you want to extract features from " + numFiles + " image(s), this could take a long time?";
                MessageBoxResult res = MessageBox.Show(dialogText, "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.OK)
                {
                    FeatureExtractionWindow extractWindow = new FeatureExtractionWindow(inputImageFilenames);
                    extractWindow.ShowDialog();

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
