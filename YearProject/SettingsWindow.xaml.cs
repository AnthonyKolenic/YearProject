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
        public SettingsWindow()
        {
            InitializeComponent();
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

            inputText = txtSeed.Text;
            if (!Int32.TryParse(inputText, out tmp))
            {
                tmp = (inputText.Equals("")) ? (int)DateTime.Now.Ticks : inputText.GetHashCode();
            }
            holder.Seed = tmp;
            System.Console.WriteLine("Na Na Na Na");
            Close();
            System.Console.WriteLine("Batman");
            GenerationWindow genWindow = new GenerationWindow(holder);
            genWindow.ShowDialog();
        }


    }
}
