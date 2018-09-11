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
    /// Interaction logic for GenrationWindow.xaml
    /// </summary>
    public partial class GenerationWindow : Window
    {
        public GenerationWindow(Settings settings, Emgu.CV.Util.VectorOfVectorOfPoint contours)
        {
            InitializeComponent();
        }

        private void CancelExtraction_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
