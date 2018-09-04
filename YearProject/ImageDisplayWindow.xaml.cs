using Emgu.CV;
using Emgu.CV.Structure;
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
    /// Interaction logic for ArtDisplayWindow.xaml
    /// </summary>
    public partial class ImageDisplayWindow : Window
    {
        public ImageDisplayWindow(Image<Bgr, Byte> img)
        {
            InitializeComponent();
            using (System.Drawing.Bitmap source = img.Bitmap)
            {
                IntPtr pointerToPixels = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    pointerToPixels,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                //TODO (Anthony): memory leak need deleteOpbejct
                Display.Source = bitmapSource;
            }
        }
    }
}
