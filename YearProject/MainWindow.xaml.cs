using Emgu.CV;
using Emgu.CV.Structure;
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

//Jeff

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
        Dictionary<String, VectorOfVectorOfPoint> features;
        VectorOfVectorOfPoint contours; 


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

        public void UpdateFeatureExtractList(Dictionary<String, VectorOfVectorOfPoint> features)
        {
            featuresExtractValid = true;
            this.features = features;
            foreach (String key in features.Keys.ToArray<String>())
            {
                lstKeypoints.Items.Add(key);
            }
        }

        private void Item_Clicked(object sender, RoutedEventArgs e)
        {
            string filename = (String)lstKeypoints.SelectedItem;
            BitmapImage canvas = new BitmapImage(new Uri(filename));
            int width = canvas.PixelWidth;
            int height = canvas.PixelHeight;
            /* 
            //Old drawing methods
            DrawingVisual renderer = new DrawingVisual();
            
            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(255,255,0)),1);
            using (DrawingContext context = renderer.RenderOpen())
            {
                context.DrawImage(canvas, new Rect(0, 0, width, height));
                
                pen = new Pen(new SolidColorBrush(Color.FromRgb(255, 0, 0)), 1);
                int counter = 0;
                foreach (LineSegment2D line in features[filename].Lines.ToArray())
                {
                    counter++;

                    context.DrawLine(pen, new System.Windows.Point(line.P1.X, line.P1.Y), new System.Windows.Point(line.P2.X, line.P2.Y));
                }
                System.Console.WriteLine("Number of lines drawn: " + counter);

                counter = 0;
                pen = new Pen(new SolidColorBrush(Color.FromRgb(255, 255, 0)), 1);
                foreach (MKeyPoint point in features[filename].KeyPoints.ToArray() )
                {
                    counter++;
                    
                    int x = (int)point.Point.X;
                    int y = (int)point.Point.Y;
                    System.Console.WriteLine("Dot at: ( " + x + " , " + y + " )");
                    context.DrawRectangle(null, pen, new Rect(x - 1,y - 1, 3, 3));
                }
                System.Console.WriteLine("Number of Points drawn: " + counter);

                

            }

            RenderTargetBitmap target = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            target.Render(renderer);

            imgKeyPoints.Source = target;
            */
            Image<Bgr, Byte> img = new Image<Bgr, byte>(filename);
           
            /*
            foreach (LineSegment2D line in features[filename].Lines)
                img.Draw(line, new Bgr(0, 0, 255.0), 1);
            */
            CvInvoke.DrawContours(img, features[filename], -1, new MCvScalar(255, 0, 255));

            using (System.Drawing.Bitmap source = img.Bitmap)
            {
                IntPtr pointerToPixels = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    pointerToPixels,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                //TODO (Anthony): memory leak need deleteOpbejct
                imgKeyPoints.Source = bitmapSource;
            }
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

        
        private void GenerateImage_Clicked(object sender, RoutedEventArgs e)
        {
            if (contours == null || contours.Size == 0)
            {
                MessageBox.Show("No objects have been extracted", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SettingsWindow settingsWIndow = new SettingsWindow(contours,features);
            settingsWIndow.ShowDialog();
            /*
                int numOfSceneObjects = 15;
                int numClones = 10;
                int numIterations = 10;
                VectorOfPoint[] objects = new VectorOfPoint[numOfSceneObjects];
                Random rand = new Random();
                for (int i = 0; i < numOfSceneObjects;i++)
                {
                    int objectIndex = rand.Next(contours.Size);
                    objects[i] = offsetContour(contours[objectIndex],new System.Drawing.Point(rand.Next(300), rand.Next(300)));
                }


                VectorOfPoint[][] clones = new VectorOfPoint[numClones][];
                GaussianRandom gRand = new GaussianRandom();
                for (int i = 0;i < numClones;i++)
                {
                    clones[i] = objects;
                }
                for (int z = 0; z < numIterations; z++)
                {
                    for (int i = 0; i < numClones; i++) // for each clone
                    {
                        //mutate clone

                        for (int k = 0; k < clones[i].Length; k++) // for each scene object
                        {
                            int numMutatePoint = clones[i][k].Size;// rand.Next((int)(clones[i][k].Size * 0.1)); // only mutate 10% of points
                            System.Console.WriteLine(numMutatePoint);
                            for (int f = 0; f < numMutatePoint; f++)
                            {
                                clones[i][k][f].Offset((int)(gRand.NextValue() * 10), (int)(gRand.NextValue() * 10));
                            }
                        }
                        //set clone to a copy of the selected
                        int selClone = rand.Next(numClones);
                        for (int k = 0; k < numClones; k++)
                        {
                            clones[k] = clones[selClone];
                        }
                    }
                }
                //TODO (Anthony): Allow different size images
                Image < Bgr, byte> img= new Image<Bgr, byte>(300, 300, new Bgr(255, 255, 255));
                CvInvoke.DrawContours(img, new VectorOfVectorOfPoint(clones[rand.Next(numClones)]), -1, new MCvScalar(0, 0, 0));
                ImageDisplayWindow displayWindow = new ImageDisplayWindow(img);
                displayWindow.ShowDialog();
            */
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            imageList.ForceClose();

        }

        public void setObjects(VectorOfVectorOfPoint contours)
        {
            this.contours = contours;  
        }

        

    }
}
