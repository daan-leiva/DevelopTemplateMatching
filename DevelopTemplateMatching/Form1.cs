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
            string image_to_analyze = "C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/test.png";
            Dictionary<int, List<Point>> locations = Source(image_to_analyze);
        }

        private Dictionary<int, List<Point>> Source(string img_location)
        {
            // create dictionary of value to template locations
            Dictionary<int, string> template_locations = new Dictionary<int, string>();
            template_locations.Add(9, @"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/9.png");
            template_locations.Add(10, @"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/10.png");
            template_locations.Add(11, @"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/11.png");
            template_locations.Add(201, @"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/201.png");
            template_locations.Add(207, @"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/207.png");
            template_locations.Add(208, @"C:/Users/Bruce Huffa/source/repos/DevelopTemplateMatching/DevelopTemplateMatching/images/208.png");

            // result dict
            Dictionary<int, List<Point>> all_matches = new Dictionary<int, List<Point>>();

            foreach (int key in template_locations.Keys)
            {
                // template matching
                List<Point> matches = Realtime_Matching(template_locations[key], img_location);
                all_matches.Add(key, matches);
            }

            // create windows to output image
            string img_win = "Outlined Image";
            CvInvoke.NamedWindow(img_win);

            // draw boxes around matches
            Image<Bgr, byte> display_img = new Image<Bgr, byte>(img_location);
            Random rand = new Random(); // random number generator to get color
            foreach (int key in all_matches.Keys)
            {
                Image<Bgr, byte> templ_img = new Image<Bgr, byte>(template_locations[key]);                
                List<Point> matches = all_matches[key];
                // get a random color to draw rectange
                int c1 = rand.Next(0, 255);
                int c2 = rand.Next(0, 255);
                int c3 = rand.Next(0, 255);

                foreach (Point match in matches)
                {
                    Rectangle match_rect = new Rectangle(match, templ_img.Size);
                    CvInvoke.Rectangle(display_img, match_rect, new MCvScalar(c1, c2, c3), 2, LineType.FourConnected, 0);
                }
            }

            // show on screen
            CvInvoke.Imshow(img_win, display_img);
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyAllWindows();

            return all_matches;
        }

        private List<Point> Realtime_Matching(string template_location, string image_location)
        {
            Image<Bgr, byte> templImage = new Image<Bgr, byte>(template_location);
            Image<Bgr, byte> testImage = new Image<Bgr, byte>(image_location);
            Image<Gray, float> result;

            using (result = testImage.MatchTemplate(templImage, Emgu.CV.CvEnum.TemplateMatchingType.CcorrNormed))
            {
                List<Point> matches = Get_Max_Above_Threshold(result);

                return matches;
            }
        }

        public static List<Point> Get_Max_Above_Threshold(Image<Gray, float> img)
        {
            List<Point> points = new List<Point>();

            double[] minValues;
            double[] maxValues;
            Point[] minLocations;
            Point[] maxLocations;

            while (true)
            {
                img.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                if (maxValues[0] < 0.999)
                    break;
                points.Add(maxLocations[0]);
                img.Data[maxLocations[0].Y, maxLocations[0].X, 0] = 0;
            }

            return points;
        }
    }
}
