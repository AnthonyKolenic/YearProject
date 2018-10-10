using System;
using System.Collections.Generic;
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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Emgu.CV.Util.VectorOfVectorOfPoint contours;
        Dictionary<String, Emgu.CV.Util.VectorOfVectorOfPoint> features;

        public SettingsWindow(Emgu.CV.Util.VectorOfVectorOfPoint contours, Dictionary<String, Emgu.CV.Util.VectorOfVectorOfPoint> features)
        {
            InitializeComponent();
            this.contours = contours;
            this.features = features;
        }
        
        private void StartGeneration_Click(object sender, RoutedEventArgs e)
        {
            Settings holder = new Settings();
            int tmp;

            string inputText = txtNumNegative.Text;
            if (!Int32.TryParse(inputText,out tmp))
            {
                MessageBoxResult res = MessageBox.Show(inputText + " is not a valid integer value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            holder.NumberOfNegativeDetectors = tmp;

            inputText = txtNumPositive.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                MessageBoxResult res = MessageBox.Show(inputText + " is not a valid integer value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            holder.NumberOfPositiveDetectors = tmp;

            /* Place Holders
            inputText = txtNumPositive.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                MessageBoxResult res = MessageBox.Show(inputText + " is not a valid integer value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            holder.NumberOfPositiveDetectors = tmp;

            inputText = txtNumPositive.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                MessageBoxResult res = MessageBox.Show(inputText + " is not a valid integer value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            holder.NumberOfPositiveDetectors = tmp;
            */

            inputText = txtWidth.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                MessageBoxResult res = MessageBox.Show(inputText + " is not a valid integer value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            holder.Width = tmp;

            inputText = txtHeight.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                MessageBoxResult res = MessageBox.Show(inputText + " is not a valid integer value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            holder.Height = tmp;

            inputText = txtNumObj.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                MessageBoxResult res = MessageBox.Show(inputText + " is not a valid integer value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            holder.NumberOfObjects = tmp;

            inputText = txtSeed.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                tmp = (inputText.Equals("")) ? (int)DateTime.Now.Ticks : inputText.GetHashCode();
            }
            holder.Seed = tmp;
            Close();
            GenerationWindow genWindow = new GenerationWindow(holder,contours,features);
            genWindow.ShowDialog();
        }


    }
}
