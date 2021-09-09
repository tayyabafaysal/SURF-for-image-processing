using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System.IO;
using System.Threading;
#if !IOS
using Emgu.CV.GPU;
#endif

namespace SURFFeature
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        String pathToOriginalImage = "";
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            LCounter = 0;

            if(openFileDialog1.FileName !=null)
            this.img_original.Image = new Bitmap(openFileDialog1.FileName);

        }
        private  void processImage()
        {
            long matchTime;

            Image<Gray, Byte> observedImage = new Image<Gray, byte>(openFileDialog1.FileName);

            String TraingSetsBasePath = @"C:\Users\TAYYABA FAISAL\Desktop\FromUmar\SURFFeature\ProvidedSet\";
            String L1TrainingSet = TraingSetsBasePath+"L1";
            List<Image<Gray, Byte>> L1TraningImages = new List<Image<Gray, Byte>>();
            foreach (var path in Directory.GetFiles(L1TrainingSet))
            {
                //Console.WriteLine(path); // full path

                Image<Gray, Byte> imgToTraing = new Image<Gray, byte>(path);
                L1TraningImages.Add(imgToTraing);

                //Console.WriteLine(System.IO.Path.GetFileName(path)); // file name
            }

            String L2TrainingSet = TraingSetsBasePath + "L2";
            List<Image<Gray, Byte>> L2TraningImages = new List<Image<Gray, Byte>>();
            foreach (var path in Directory.GetFiles(L2TrainingSet))
            {
                //Console.WriteLine(path); // full path

                Image<Gray, Byte> imgToTraing = new Image<Gray, byte>(path);
                L2TraningImages.Add(imgToTraing);

                //Console.WriteLine(System.IO.Path.GetFileName(path)); // file name
            }


            String L3TrainingSet = TraingSetsBasePath + "L3";
            List<Image<Gray, Byte>> L3TraningImages = new List<Image<Gray, Byte>>();
            foreach (var path in Directory.GetFiles(L3TrainingSet))
            {
                //Console.WriteLine(path); // full path

                Image<Gray, Byte> imgToTraing = new Image<Gray, byte>(path);
                L3TraningImages.Add(imgToTraing);

                //Console.WriteLine(System.IO.Path.GetFileName(path)); // file name
            }

            String L4TrainingSet = TraingSetsBasePath + "L4";
            List<Image<Gray, Byte>> L4TraningImages = new List<Image<Gray, Byte>>();
            foreach (var path in Directory.GetFiles(L4TrainingSet))
            {
                //Console.WriteLine(path); // full path

                Image<Gray, Byte> imgToTraing = new Image<Gray, byte>(path);
                L4TraningImages.Add(imgToTraing);

                //Console.WriteLine(System.IO.Path.GetFileName(path)); // file name
            }






            Image<Bgr, byte> result = Draw(openFileDialog1.FileName, L1TraningImages, L2TraningImages, L3TraningImages, L4TraningImages, observedImage, out matchTime, new Label(), 1);



            //ImageViewer.Show(result, String.Format("Matched using {0} in {1} milliseconds", GpuInvoke.HasCuda ? "GPU" : "CPU", matchTime));
            System.Drawing.Image result1 = result.ToBitmap();
            img_processed.Image = result1;

            /*


            String pathStr = @"C:\Emgu\emgucv-windows-universal-cuda 2.4.10.1940\Emgu.CV.Example\SURFFeature\NewTraningData\\";

            using (Image<Gray, Byte> modelImage1a = new Image<Gray, byte>(pathStr + "L1a.jpg"))
            using (Image<Gray, Byte> modelImage1b = new Image<Gray, byte>(pathStr + "L1b.jpg"))
            using (Image<Gray, Byte> modelImage1c = new Image<Gray, byte>(pathStr + "L1c.jpg"))
            using (Image<Gray, Byte> modelImage2a = new Image<Gray, byte>(pathStr + "L2a.jpg"))
            using (Image<Gray, Byte> modelImage2b = new Image<Gray, byte>(pathStr + "L2b.jpg"))
            using (Image<Gray, Byte> modelImage2c = new Image<Gray, byte>(pathStr + "L2c.jpg"))
            using (Image<Gray, Byte> modelImage4a = new Image<Gray, byte>(pathStr + "L4a.jpg"))
            using (Image<Gray, Byte> modelImage4b = new Image<Gray, byte>(pathStr + "L4b.jpg"))
            using (Image<Gray, Byte> modelImage4c = new Image<Gray, byte>(pathStr + "L4c.jpg"))
            using (Image<Gray, Byte> modelImage3a = new Image<Gray, byte>(pathStr + "L3a.jpg"))
            using (Image<Gray, Byte> modelImage3b = new Image<Gray, byte>(pathStr + "L3b.jpg"))
            using (Image<Gray, Byte> modelImage3c = new Image<Gray, byte>(pathStr + "L3c.jpg"))


                
            using (Image<Gray, Byte> observedImage4 = new Image<Gray, byte>(openFileDialog1.FileName))
            {
                Image<Bgr, byte> result4 = Draw(openFileDialog1.FileName, modelImage1a, modelImage1b, modelImage1c,
                    modelImage2a, modelImage2b, modelImage2c,
                    modelImage3a, modelImage3b, modelImage3c,
                    modelImage4a, modelImage4b, modelImage4c, observedImage, out matchTime);
                //ImageViewer.Show(result, String.Format("Matched using {0} in {1} milliseconds", GpuInvoke.HasCuda ? "GPU" : "CPU", matchTime));
                System.Drawing.Image result5 = result4.ToBitmap();
                pictureBox1.Image = result5;
            }
            */
        }

        public static void FindMatch(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, out Matrix<int> indices, out Matrix<byte> mask, out HomographyMatrix homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.8;
            SURFDetector surfCPU = new SURFDetector(500, false);
            Stopwatch watch;
            homography = null;
#if !IOS
            if (GpuInvoke.HasCuda)
            {
                GpuSURFDetector surfGPU = new GpuSURFDetector(surfCPU.SURFParams, 0.01f);
                using (GpuImage<Gray, Byte> gpuModelImage = new GpuImage<Gray, byte>(modelImage))
                //extract features from the object image
                using (GpuMat<float> gpuModelKeyPoints = surfGPU.DetectKeyPointsRaw(gpuModelImage, null))
                using (GpuMat<float> gpuModelDescriptors = surfGPU.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                using (GpuBruteForceMatcher<float> matcher = new GpuBruteForceMatcher<float>(DistanceType.L2))
                {
                    modelKeyPoints = new VectorOfKeyPoint();
                    surfGPU.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    using (GpuImage<Gray, Byte> gpuObservedImage = new GpuImage<Gray, byte>(observedImage))
                    using (GpuMat<float> gpuObservedKeyPoints = surfGPU.DetectKeyPointsRaw(gpuObservedImage, null))
                    using (GpuMat<float> gpuObservedDescriptors = surfGPU.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                    using (GpuMat<int> gpuMatchIndices = new GpuMat<int>(gpuObservedDescriptors.Size.Height, k, 1, true))
                    using (GpuMat<float> gpuMatchDist = new GpuMat<float>(gpuObservedDescriptors.Size.Height, k, 1, true))
                    using (GpuMat<Byte> gpuMask = new GpuMat<byte>(gpuMatchIndices.Size.Height, 1, 1))
                    using (Emgu.CV.GPU.Stream stream = new Emgu.CV.GPU.Stream())
                    {
                        matcher.KnnMatchSingle(gpuObservedDescriptors, gpuModelDescriptors, gpuMatchIndices, gpuMatchDist, k, null, stream);
                        indices = new Matrix<int>(gpuMatchIndices.Size);
                        mask = new Matrix<byte>(gpuMask.Size);

                        //gpu implementation of voteForUniquess
                        using (GpuMat<float> col0 = gpuMatchDist.Col(0))
                        using (GpuMat<float> col1 = gpuMatchDist.Col(1))
                        {
                            GpuInvoke.Multiply(col1, new MCvScalar(uniquenessThreshold), col1, stream);
                            GpuInvoke.Compare(col0, col1, gpuMask, CMP_TYPE.CV_CMP_LE, stream);
                        }

                        observedKeyPoints = new VectorOfKeyPoint();
                        surfGPU.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                        //wait for the stream to complete its tasks
                        //We can perform some other CPU intesive stuffs here while we are waiting for the stream to complete.
                        stream.WaitForCompletion();

                        gpuMask.Download(mask);
                        gpuMatchIndices.Download(indices);

                        if (GpuInvoke.CountNonZero(gpuMask) >= 4)
                        {
                            int nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                            if (nonZeroCount >= 4)
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                        }

                        watch.Stop();
                    }
                }
            }
            else
#endif
            {
                //extract features from the object image
                modelKeyPoints = new VectorOfKeyPoint();
                Matrix<float> modelDescriptors = surfCPU.DetectAndCompute(modelImage, null, modelKeyPoints);

                watch = Stopwatch.StartNew();

                // extract features from the observed image
                observedKeyPoints = new VectorOfKeyPoint();
                Matrix<float> observedDescriptors = surfCPU.DetectAndCompute(observedImage, null, observedKeyPoints);
                BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
                matcher.Add(modelDescriptors);

                indices = new Matrix<int>(observedDescriptors.Rows, k);
                using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
                {
                    matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
                    mask = new Matrix<byte>(dist.Rows, 1);
                    mask.SetValue(255);
                    Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
                }

                int nonZeroCount = CvInvoke.cvCountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                }

                watch.Stop();
            }
            matchTime = watch.ElapsedMilliseconds;
        }

        public static void FindMatch_(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, out Matrix<int> indices, out Matrix<byte> mask, out HomographyMatrix homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.5;
            SURFDetector surfCPU = new SURFDetector(700, false);
            Stopwatch watch;
            homography = null;
#if !IOS
            if (GpuInvoke.HasCuda)
            {
                GpuSURFDetector surfGPU = new GpuSURFDetector(surfCPU.SURFParams, 0.01f);
                using (GpuImage<Gray, Byte> gpuModelImage = new GpuImage<Gray, byte>(modelImage))
                //extract features from the object image
                using (GpuMat<float> gpuModelKeyPoints = surfGPU.DetectKeyPointsRaw(gpuModelImage, null))
                using (GpuMat<float> gpuModelDescriptors = surfGPU.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                using (GpuBruteForceMatcher<float> matcher = new GpuBruteForceMatcher<float>(DistanceType.L2))
                {
                    modelKeyPoints = new VectorOfKeyPoint();
                    surfGPU.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    using (GpuImage<Gray, Byte> gpuObservedImage = new GpuImage<Gray, byte>(observedImage))
                    using (GpuMat<float> gpuObservedKeyPoints = surfGPU.DetectKeyPointsRaw(gpuObservedImage, null))
                    using (GpuMat<float> gpuObservedDescriptors = surfGPU.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                    using (GpuMat<int> gpuMatchIndices = new GpuMat<int>(gpuObservedDescriptors.Size.Height, k, 1, true))
                    using (GpuMat<float> gpuMatchDist = new GpuMat<float>(gpuObservedDescriptors.Size.Height, k, 1, true))
                    using (GpuMat<Byte> gpuMask = new GpuMat<byte>(gpuMatchIndices.Size.Height, 1, 1))
                    using (Emgu.CV.GPU.Stream stream = new Emgu.CV.GPU.Stream())
                    {
                        matcher.KnnMatchSingle(gpuObservedDescriptors, gpuModelDescriptors, gpuMatchIndices, gpuMatchDist, k, null, stream);
                        indices = new Matrix<int>(gpuMatchIndices.Size);
                        mask = new Matrix<byte>(gpuMask.Size);

                        //gpu implementation of voteForUniquess
                        using (GpuMat<float> col0 = gpuMatchDist.Col(0))
                        using (GpuMat<float> col1 = gpuMatchDist.Col(1))
                        {
                            GpuInvoke.Multiply(col1, new MCvScalar(uniquenessThreshold), col1, stream);
                            GpuInvoke.Compare(col0, col1, gpuMask, CMP_TYPE.CV_CMP_LE, stream);
                        }

                        observedKeyPoints = new VectorOfKeyPoint();
                        surfGPU.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                        //wait for the stream to complete its tasks
                        //We can perform some other CPU intesive stuffs here while we are waiting for the stream to complete.
                        stream.WaitForCompletion();

                        gpuMask.Download(mask);
                        gpuMatchIndices.Download(indices);

                        if (GpuInvoke.CountNonZero(gpuMask) >= 4)
                        {
                            int nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                            if (nonZeroCount >= 4)
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                        }

                        watch.Stop();
                    }
                }
            }
            else
#endif
            {
                //extract features from the object image
                modelKeyPoints = new VectorOfKeyPoint();
                Matrix<float> modelDescriptors = surfCPU.DetectAndCompute(modelImage, null, modelKeyPoints);

                watch = Stopwatch.StartNew();

                // extract features from the observed image
                observedKeyPoints = new VectorOfKeyPoint();
                Matrix<float> observedDescriptors = surfCPU.DetectAndCompute(observedImage, null, observedKeyPoints);
                BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
                matcher.Add(modelDescriptors);

                indices = new Matrix<int>(observedDescriptors.Rows, k);
                using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
                {
                    try
                    {
                        matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
                    }
                    catch (Exception ex)
                    { }
                    mask = new Matrix<byte>(dist.Rows, 1);
                    mask.SetValue(255);
                    Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
                }

                int nonZeroCount = CvInvoke.cvCountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                }

                watch.Stop();
            }
            matchTime = watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Draw the model image and observed image, the matched features and homography projection.
        /// </summary>
        /// <param name="modelImage">The model image</param>
        /// <param name="observedImage">The observed image</param>
        /// <param name="matchTime">The output total time for computing the homography matrix.</param>
        /// <returns>The model image and observed image, the matched features and homography projection.</returns>
        public static Image<Bgr, Byte> Draw(String filename, Image<Gray, Byte> modelImage1a, Image<Gray, Byte> modelImage1b, Image<Gray, Byte> modelImage1c,
            Image<Gray, Byte> modelImage2a, Image<Gray, Byte> modelImage2b, Image<Gray, Byte> modelImage2c,
            Image<Gray, Byte> modelImage3a, Image<Gray, Byte> modelImage3b, Image<Gray, Byte> modelImage3c,
            Image<Gray, Byte> modelImage4a, Image<Gray, Byte> modelImage4b, Image<Gray, Byte> modelImage4c, Image<Gray, byte> observedImage, out long matchTime)
        {
            HomographyMatrix homography1;
            HomographyMatrix homography2;
            HomographyMatrix homography3;
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;
            Matrix<byte> mask1;
            Matrix<byte> mask2;
            Matrix<byte> mask3;
            int mask1Count;
            int mask2Count;
            int mask3Count;

            //Draw the matched keypoints
            Image<Bgr, Byte> result = new Image<Bgr, byte>(filename);

            #region L1 Segment
            FindMatch(modelImage1a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage1b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage1c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);
            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage1a.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L1", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage1b.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L1", ref f, pts1[0], new Bgr(Color.Black));
                    
                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage1c.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L1", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion

            #region L2 Segment
            FindMatch(modelImage2a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage2b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage2c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);

            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage2a.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L2", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage2b.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L2", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage2c.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L2", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion

            #region L4 Segment
            FindMatch(modelImage4a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage4b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage4c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);

            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage4a.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L4", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage4b.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L4", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage4c.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L4", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion

            #region L3 Segment
            FindMatch(modelImage3a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage3b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage3c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);

            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage3a.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L3", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage3b.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L3", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage3c.ROI;
                    PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L3", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion

            return result;
        }

        static int LCounter = 0;


        public static double Distance(Point p3, Point p4)
        {

            int dx = p3.X - p4.X;
            int dy = p3.Y - p4.Y;

            return Math.Sqrt(dx * dx + dy * dy);
            

        }


        int totalProcessed = 0;
        public  Image<Bgr, Byte> Draw(String filename, List<Image<Gray, byte>> L1TraningImages, List<Image<Gray, byte>> L2TraningImages, List<Image<Gray, byte>> L3TraningImages, List<Image<Gray, byte>> L4TraningImages,   Image<Gray, byte> observedImage, out long matchTime, Label lbl, int lNumber)
        {
            totalProcessed = 0;
            Image<Bgr, Byte> result = new Image<Bgr, byte>(filename);


            int totalToBeProessed = L1TraningImages.Count + L2TraningImages.Count + L3TraningImages.Count + L4TraningImages.Count;




            int x = 1;
            for (x = 1; x <= 4; x++)
            {
                
                if (x == 2)
                    L1TraningImages = L2TraningImages;
                if (x == 3)
                    L1TraningImages = L3TraningImages;
                if (x == 4)
                    L1TraningImages = L4TraningImages;

                List<HomographyMatrix> homographyList = new List<HomographyMatrix>();


                VectorOfKeyPoint modelKeyPoints;
                VectorOfKeyPoint observedKeyPoints;
                Matrix<int> indices;


                List<Matrix<byte>> masksList = new List<Matrix<byte>>();
                List<int> masksCount = new List<int>();


                //Draw the matched keypoints



                int matchCounter = 0;
                Matrix<byte> tempMask;
                HomographyMatrix tempHomeGraphy;

                foreach (Image<Gray, byte> traniningImage in L1TraningImages)
                {
                    totalProcessed++;

                    Console.WriteLine(totalProcessed +" - "+totalToBeProessed + " - "+ (totalProcessed/totalToBeProessed));
                    int percentage =(int) (((totalProcessed * 1.0) / (totalToBeProessed * 1.0)) * 100) ;
                    lblPer.Text = "ProcessStatus : "+ percentage + "%";
                    progressBar1.Value = percentage;
                    Application.DoEvents();
                    Console.WriteLine(traniningImage.Size + "Matchin " + observedImage.Size + "with Image number " + matchCounter++);

                    FindMatch(traniningImage, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out tempMask, out tempHomeGraphy);

                    homographyList.Add(tempHomeGraphy);
                    masksList.Add(tempMask);
                    masksCount.Add(CvInvoke.cvCountNonZero(tempMask));
                    /*
                    int greatestMaskCountIndex = masksCount.IndexOf(masksCount.Max());

                    {  //draw a rectangle along the projected model
                        Rectangle rect = imgsList.ElementAt(greatestMaskCountIndex).ROI;
                        PointF[] pts = new PointF[] {
                   new PointF(rect.Left, rect.Bottom),
                   new PointF(rect.Right, rect.Bottom),
                   new PointF(rect.Right, rect.Top),
                   new PointF(rect.Left, rect.Top)};
                        homographyList.ElementAt(greatestMaskCountIndex).ProjectPoints(pts);

                        Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                        result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                        MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                        result.Draw("L" + (++LCounter), ref f, pts1[0], new Bgr(Color.Black));
                    }


                    */


                }


                int i = lNumber;
                // for (int i= 0; i< homographyList.Count;i++)
                {
                    foreach (int n in masksCount)
                    {
                        Console.Write(n + " ");
                    }

                    Console.WriteLine();
                    NotRectangleFound:
                    int greatestMaskCountIndex = masksCount.IndexOf(masksCount.Max());

                    if (homographyList[greatestMaskCountIndex] != null)
                    {  //draw a rectangle along the projected model

                        Console.Write(i + "Homo List Size " + homographyList.Count + " Max Value :: " + masksCount.Max() + " Index :: " + masksCount.IndexOf(masksCount.Max()));

                        Rectangle rect = L1TraningImages.ElementAt(greatestMaskCountIndex).ROI;
                        PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};

                        
                        homographyList.ElementAt(greatestMaskCountIndex).ProjectPoints(pts);



                        Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);

                        if (3 == 4)
                        {
                            masksCount.RemoveAt(greatestMaskCountIndex);
                            L1TraningImages.RemoveAt(greatestMaskCountIndex);
                            goto NotRectangleFound;
                        }
                        
                        Console.WriteLine("Points Length in X  "+x+"is "+pts1.Count());
                        foreach (Point p in pts1)
                        {
                            
                            Console.WriteLine(">>>"+p.X+"," +p.Y+"-"+pts1.Count());
                        }

                        //pts1 contains points (x,y) in this order: lowerLeft-LowerRight-UpperRight-UpperLeft
                        Point lowerMid = new Point();
                        lowerMid.X = ((pts1[1].X - pts1[0].X)/2)+ pts1[0].X;
                        lowerMid.Y = pts1[1].Y;

                        

                        Point upperMid = new Point();
                        upperMid.X = ((pts1[2].X - pts1[3].X)/2)+pts1[3].X;
                        upperMid.Y = pts1[2].Y;

                        Double distanceBWCurrentVer = Distance(upperMid, lowerMid);
                        

                        

                        int crossHeight = 20;
                        int crossWidth = 20;
                        Bgr crossColor = new Bgr(Color.Azure);
                        int crossThickness = 5;
                        result.Draw(new Cross2DF(lowerMid, crossHeight, crossWidth), crossColor, crossThickness);
                        img_processed.Image = result.ToBitmap();
                        Application.DoEvents();
                        Thread.Sleep(40);
                        result.Draw(new Cross2DF(upperMid, crossHeight, crossWidth), crossColor, crossThickness);
                        img_processed.Image = result.ToBitmap();
                        Application.DoEvents();
                        Thread.Sleep(40);
                        foreach (Point p in pts1)
                        {
                            result.Draw(new Cross2DF(p, crossHeight, crossWidth), crossColor, crossThickness);
                            img_processed.Image = result.ToBitmap();
                            Thread.Sleep(40);
                            Application.DoEvents();
                            result.Draw(new Cross2DF(p, crossHeight, crossWidth), crossColor, crossThickness);
                            img_processed.Image = result.ToBitmap();
                            Thread.Sleep(40);
                            Application.DoEvents();
                            result.Draw(new Cross2DF(p, crossHeight, crossWidth), crossColor, crossThickness); result.Draw(new Cross2DF(lowerMid, crossHeight, crossWidth), crossColor, crossThickness);
                            img_processed.Image = result.ToBitmap();
                            Thread.Sleep(40);
                            Application.DoEvents();
                            result.Draw(new Cross2DF(p, crossHeight, crossWidth), crossColor, crossThickness);
                            img_processed.Image = result.ToBitmap();
                            Thread.Sleep(40);
                            Application.DoEvents();
                        }

                        result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                        MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 1.2, 3.6);
                        result.Draw("L" + (x) + ":" + getStrFromDistance(distanceBWCurrentVer), ref f, new Point(pts1[0].X + 5, pts1[0].Y - 5), new Bgr(Color.Blue));
                        img_processed.Image = result.ToBitmap();
                        Thread.Sleep(40);
                        Application.DoEvents();

                    }



                }

                               
                Console.WriteLine("Value of LConunter " + LCounter);
            }
            matchTime = 0;
            return result;
            /*
            #region L1 Segment
            FindMatch(modelImage1a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage1b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage1c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);

            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage1a.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L1", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage1b.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L1", ref f, pts1[0], new Bgr(Color.Black));

                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage1c.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L1", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion

            #region L2 Segment
            FindMatch(modelImage2a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage2b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage2c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);

            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage2a.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L2", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage2b.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L2", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage2c.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L2", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion

            #region L4 Segment
            FindMatch(modelImage4a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage4b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage4c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);

            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage4a.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L4", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage4b.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L4", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage4c.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L4", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion

            #region L3 Segment
            FindMatch(modelImage3a, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask1, out homography1);
            FindMatch(modelImage3b, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask2, out homography2);
            FindMatch(modelImage3c, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, out indices, out mask3, out homography3);

            mask1Count = CvInvoke.cvCountNonZero(mask1);
            mask2Count = CvInvoke.cvCountNonZero(mask2);
            mask3Count = CvInvoke.cvCountNonZero(mask3);

            if (mask1Count > mask2Count && mask1Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography1 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage3a.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography1.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Red), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L3", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else if (mask2Count > mask1Count && mask2Count > mask3Count)
            {
                #region draw the projected region on the image
                if (homography2 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage3b.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography2.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L3", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion
            }
            else
            {
                #region draw the projected region on the image
                if (homography3 != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage3c.ROI;
                    PointF[] pts = new PointF[] {
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                    homography3.ProjectPoints(pts);

                    Point[] pts1 = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    result.DrawPolyline(pts1, true, new Bgr(Color.Green), 5);
                    MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.2, 4.6);
                    result.Draw("L3", ref f, pts1[0], new Bgr(Color.Black));
                }
                #endregion

            }
            #endregion
            */

        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

                processImage();
            
            
            
        }
        private void callbackfunction(IAsyncResult res)
        {
            // it will be called when your job finishes.
        }

        private void saveProImage_Click(object sender, EventArgs e)
        {
            this.img_processed.Image.Save(@"D:\ProcessedImage.jpg");
            MessageBox.Show("Processed Image Store to D:\\ProcessedImage.jpg ");
        }
        private string getStrFromDistance(double distance)
        {
            if (distance == 68.0 && distance <= 74.0)
            {
                return "BICONCAVE";
            }
            else if (distance == 40.0 && distance <= 58.0)
            {
                return  "WEDGE";
            }
            else if (distance == 0.0 && distance <= 39.0)
            {
                return  "CRUSh";
            }

            else
            {
               return  "NORMAL";
            }
        }
    }
}
