﻿using System;
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
        private DataPass information;
        public GenerationWindow(Settings settings, Emgu.CV.Util.VectorOfVectorOfPoint contours,Dictionary<String, Emgu.CV.Util.VectorOfVectorOfPoint> features)
        {
            InitializeComponent();
            generatorThread = new BackgroundWorker();
            generatorThread.WorkerReportsProgress = true;
            generatorThread.WorkerSupportsCancellation = true;
            generatorThread.DoWork += new DoWorkEventHandler(Generator_DoWork);
            generatorThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Generator_RunWorkerCompleted);
            generatorThread.ProgressChanged += new ProgressChangedEventHandler(Generator_ProgressChanged);
            DataPass tmp = new DataPass
            {
                settings = settings,
                objects = contours,
                inputData = features
            };
            information = tmp;
            generatorThread.RunWorkerAsync(tmp);

        }

        private void CancelExtraction_Click(object sender, RoutedEventArgs e)
        {
            generatorThread.CancelAsync();
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
                Emgu.CV.Util.VectorOfVectorOfPoint temp = (Emgu.CV.Util.VectorOfVectorOfPoint)e.Result;
                Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> img = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(information.settings.Width, information.settings.Height, new Emgu.CV.Structure.Bgr(255, 255, 255));
                Emgu.CV.CvInvoke.DrawContours(img,temp, -1, new Emgu.CV.Structure.MCvScalar(0, 0, 0));
                ImageDisplayWindow displayWindow = new ImageDisplayWindow(img);
                displayWindow.ShowDialog();
                Close();
            }
        }

        private void Generator_DoWork(object sender, DoWorkEventArgs e)
        {

            DataPass data = (DataPass)e.Argument;
            //e.Result = result;
            Random randomGen = new Random(data.settings.Seed); ;
            GaussianRandom gausRandomGen = new GaussianRandom(data.settings.Seed);

            Dictionary<String, Detector[]> detectors = new Dictionary<string, Detector[]>();

            //Generate detectors
            foreach (String currentImage in data.inputData.Keys)
            {
                detectors.Add(currentImage, new Detector[data.settings.NumberOfNegativeDetectors]);
                
                for (int i = 0;i < data.settings.NumberOfNegativeDetectors;i++)
                {
                    int attempts = 0;
                    bool found = false;
                    while (!found && attempts < 10)
                    {
                        attempts++; // To prevent infinite loop
                        //TODO (Anthony): Width and height should be individual per image
                        int randomX = randomGen.Next(0, data.settings.Width);
                        int randomY = randomGen.Next(0, data.settings.Height);
                        //TODO (Anthony): Setting perhaps?
                        int radius = randomGen.Next(10, 50);
                        int doubleRadius = radius * radius;
                        Detector newDetector = new Detector(randomX,randomY,radius);

                        int numDetectSelf = 0;
                        //check for collisions with points in each image

                        foreach (System.Drawing.Point[] currentObjectList in data.inputData[currentImage].ToArrayOfArray()) // loop over each object in image
                        {
                            foreach (System.Drawing.Point point in currentObjectList) // loop over each point
                            {
                                //check if detect self
                                if (Math.Pow(point.X - randomX,2) + Math.Pow(point.Y - randomY, 2) < doubleRadius)
                                {
                                    numDetectSelf++;
                                }
                            }
                        }
                        //TODO (Anthony): Detting for maximum self detection  
                        if (numDetectSelf < 10)
                        {
                            found = true;
                            detectors[currentImage][i] = newDetector;
                        }

                        /*
                        //check for overlap with other detectors
                        for (int k = 0; k < i;k++)
                        {

                        }
                        */
                    }
                }
            }

            //start generation and mutation

            //Step 1: generate initial
            //Step 2: mutate according to affinities
            //Step 3: calculate affinities
            //Step 4: Select best
            //Step 5: go to step 2 and repeat

            BoundBox[] sizes = new BoundBox[data.objects.Size];
            for (int i = 0; i < sizes.Length;i++)
            {
                sizes[i] = ImageManipulation.getBoundBox(data.objects[i]);
            }

             
            int numImagesClones = 20;
            //list of integer and vector of vector of points, where int is the affinities and VectorOfVectorOfPoint is each objects in scene
            List<Tuple<int, Emgu.CV.Util.VectorOfVectorOfPoint>> imagesObjects = new List<Tuple<int, Emgu.CV.Util.VectorOfVectorOfPoint>>();
            Emgu.CV.Util.VectorOfPoint[] objects = new Emgu.CV.Util.VectorOfPoint[data.settings.NumberOfObjects];
            for (int k = 0; k < numImagesClones;k++)
            {
                //generate image with affinity of zero
                int affinity = 0;
                for (int i = 0; i < data.settings.NumberOfObjects; i++)
                {
                    int objectIndex = randomGen.Next(data.objects.Size);
                    int shiftX = Math.Max(0, data.settings.Width - sizes[objectIndex].Width);
                    int shiftY = Math.Max(0, data.settings.Height - sizes[objectIndex].Height);
                    objects[i] = ImageManipulation.offsetContour(data.objects[objectIndex], new System.Drawing.Point(randomGen.Next(shiftX), randomGen.Next(shiftY)));
                }
                imagesObjects.Add(new Tuple<int, Emgu.CV.Util.VectorOfVectorOfPoint>(affinity,new Emgu.CV.Util.VectorOfVectorOfPoint(objects)));
            }

            int maxIterations = 5;
            int loopCounter = 0;
            bool perfected = false;
            while (loopCounter < maxIterations && !perfected)
            {
                loopCounter++;
                //calculate affinities
                for (int i = 0; i < numImagesClones; i++) // loop through each clone
                {
                    int affinity = 0;
                    //TODO (Anthony): Check affinities for all images 
                    for (int k =0; k < imagesObjects[i].Item2.Size;k++) // loop through each object
                    {
                        for (int f = 0; f < imagesObjects[i].Item2[k].Size; f++) // loop through each point
                        {
                            foreach (Detector[] detectorsOfEachImage in detectors.Values)
                            {
                                for (int c = 0; c < detectorsOfEachImage.Length; c++)
                                    affinity += checkDistance(detectorsOfEachImage[c], imagesObjects[i].Item2[k][f]);
                            }
                        }
                    }
                    imagesObjects[i] = new Tuple<int, Emgu.CV.Util.VectorOfVectorOfPoint>(affinity, imagesObjects[i].Item2);  
                }
                //mutate according to affinity
                for (int i = 0; i < numImagesClones; i++) // loop through each clone
                {
                    int affinity = imagesObjects[i].Item1;
                    //TODO (Anthony): Check affinities for all images 
                    for (int k = 0; k < imagesObjects[i].Item2.Size; k++) // loop through each object
                    {
                        for (int f = 0; f < imagesObjects[i].Item2[k].Size; f++) // loop through each point
                        {
                            int x = (int)(gausRandomGen.NextValue() * affinity);
                            int y = (int)(gausRandomGen.NextValue() * affinity);
                            imagesObjects[i].Item2[k][f].Offset(x, y);
                        }
                    }
                   
                }
                //replace bad ones

            }

            e.Result = imagesObjects[randomGen.Next(numImagesClones)].Item2;

        }

        private int checkDistance(Detector detector,System.Drawing.Point point)
        {
            double distance = Math.Pow(detector.Location.X - point.X, 2) + Math.Pow(detector.Location.Y - point.Y, 2);
            if (distance > detector.Radius * detector.Radius)
                return 0;
            else
                return (int)((1.0 - (Math.Sqrt(distance) / detector.Radius)) * detector.Radius);
        }

        private void Generator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

    }
}
