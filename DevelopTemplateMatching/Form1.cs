using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace DevelopTemplateMatching
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Source();
        }

        private void Source()
        {
            string templ_win = "Number Image";
            string img_win = "Test Image";
            string result_win = "Result Image";
            CvInvoke.NamedWindow(templ_win); // creates window for template number
            CvInvoke.NamedWindow(img_win); // creates window for real image
            CvInvoke.NamedWindow(result_win);

            // load mats
            Mat tmpl_img = new Mat("C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/test_match3.png");
            Mat img = new Mat("C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/test.png");
            Mat result_img = new Mat();

            // template matching
            //MatchingMethod(img, tmpl_img, result_img, TemplateMatchingType.Ccorr, img_win, result_win, templ_win);
            Realtime_Matching(img_win, result_win, templ_win);

            CvInvoke.WaitKey(0);
            CvInvoke.DestroyAllWindows();
        }

        private void Sample()
        {
            String win1 = "Test Window"; //The name of the window
            CvInvoke.NamedWindow(win1); //Create the window using the specific name

            Mat img = new Mat(200, 400, DepthType.Cv8U, 3); //Create a 3 channel image of 400x200
            img.SetTo(new Bgr(255, 0, 0).MCvScalar); // set it to Blue color

            //Draw "Hello, world." on the image using the specific font
            CvInvoke.PutText(
               img,
               "Hello, world",
               new System.Drawing.Point(10, 80),
               FontFace.HersheyComplex,
               1.0,
               new Bgr(0, 255, 0).MCvScalar);


            CvInvoke.Imshow(win1, img); //Show the image
            CvInvoke.WaitKey(0);  //Wait for the key pressing event
            CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
        }

        private void MatchingMethod(Mat img, Mat templ, Mat result, TemplateMatchingType match_method, string image_window, string result_window, string temp_window)
        {
            // Source image to display
            Mat img_display = new Mat();
            img.CopyTo(img_display);

            // Create the result matrix
            int result_cols = img.Cols - templ.Cols + 1;
            int result_rows = img.Rows - templ.Rows + 1;

            result.Create(result_rows, result_cols, DepthType.Cv32F, img.NumberOfChannels);

            // Do the Matching and Normalize
            CvInvoke.MatchTemplate(img, templ, result, match_method);
            CvInvoke.Normalize(result, result, 0, 1, NormType.MinMax, DepthType.Default/*, new Mat()*/);

            // Localizing the best match with minMaxLoc
            double minVal = 0;
            double maxVal = 0;
            Point minLoc = new Point();
            Point maxLoc = new Point();
            Point matchLoc = new Point();

            CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc/*, new Mat()*/);

            // For SQDIFF and SQDIFF_NORMED, the best matches are lower values. For all the other methods, the higher the better
            if (match_method == TemplateMatchingType.Sqdiff || match_method == TemplateMatchingType.SqdiffNormed)
            {
                matchLoc = minLoc;
            }
            else
            {
                matchLoc = maxLoc;
            }

            // Show me what you got
            Rectangle rect = new Rectangle(matchLoc.X, matchLoc.Y, templ.Cols, templ.Rows);
            CvInvoke.Rectangle(img_display, rect, new MCvScalar(0), 2, LineType.EightConnected, 0);
            CvInvoke.Rectangle(result, rect, new MCvScalar(0), 2, LineType.EightConnected, 0);

            CvInvoke.Imshow(image_window, img);
            CvInvoke.Imshow(result_window, result);
            CvInvoke.Imshow(temp_window, templ);

            return;
        }

        private int Realtime_Matching(string image_window, string result_window, string temp_window)
        {
            Image<Bgr, byte> templImage = new Image<Bgr, byte>(@"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/9_crop.png");
            Image<Bgr, byte> testImage = new Image<Bgr, byte>(@"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/test.png");
            Image<Gray, float> result;
            Image<Bgr, byte> img_display = new Image<Bgr, byte>(@"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/test.png");

            using (result = testImage.MatchTemplate(templImage, Emgu.CV.CvEnum.TemplateMatchingType.CcorrNormed))
            {
                double[] minValues;
                double[] maxValues;
                Point[] minLocations;
                Point[] maxLocations;

                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > 0.8)
                {
                    Rectangle match = new Rectangle(maxLocations[0], templImage.Size);

                    CvInvoke.Rectangle(img_display, match, new MCvScalar(0), 2, LineType.EightConnected, 0);

                    MessageBox.Show(string.Join(", ",ImageToByte(result.Bitmap)));
                    result.ThresholdToZero(new Gray(0.999));

                    CvInvoke.Imshow(result_window, result);
                    CvInvoke.Imshow(image_window, img_display);
                    CvInvoke.Imshow(temp_window, templImage);

                    return 1;
                }
                else
                {
                    MessageBox.Show("Match Not Detecting");

                    return 0;
                }
            }
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
