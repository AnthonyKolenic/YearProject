using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Forms;

namespace YearProject
{
    /// <summary>
    /// Interaction logic for ImageListWindow.xaml
    /// </summary>
    public partial class ImageListWindow : Window
    {
        bool forceClose = false;
        MainWindow main;

        public ImageListWindow(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        public void ForceClose()
        {
            forceClose = true;
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!forceClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void Item_Clicked(object sender, RoutedEventArgs e)
        {
            imgDisplay.Source = new BitmapImage(new Uri((String)NameList.SelectedItem));
               
        }

        private void AddItems_Clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openDlg = new System.Windows.Forms.OpenFileDialog();
            openDlg.Multiselect = true;
            openDlg.Filter = "Image files (*.bmp,*.png)|*.bmp;*.png|All files (*.*)|*.*";
            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                NameList.Items.Clear();
                foreach (String filename in openDlg.FileNames)
                {
                    NameList.Items.Add(filename);
                }
            }
            main.UpdateImageList(GetNames());
        }

        private void RemoveItems_Clicked(object sender, RoutedEventArgs e)
        {
            while (NameList.SelectedItems.Count > 0)
            {
                NameList.Items.Remove(NameList.SelectedItems[0]);
            }
            main.UpdateImageList(GetNames());
        }

        private List<String> GetNames()
        {
            List<String> result = new List<string>();
            foreach (String filename in NameList.Items)
                result.Add(filename);
            return result;
        }

    }
}
