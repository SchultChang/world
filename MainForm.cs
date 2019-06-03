
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;

namespace MultiFaceRec
{
    public partial class FrmPrincipal : Form
    {
        //Declararation of all variables, vectors and haarcascades
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        Boolean flag = false;
        EventHandler handler;
        HaarCascade face;
       // HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        Image<Bgr, byte> imgInput;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels= new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;

        public FrmPrincipal()
        {
            InitializeComponent();
            //Load haarcascades for face detection
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            //eye = new HaarCascade("haarcascade_eye.xml");
            try
            {
                //Load of previus trainned faces and labels for each image
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels+1; tf++)
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));
                    labels.Add(Labels[tf]);
                }
            
            }
            catch(Exception)
            {
                //MessageBox.Show(e.ToString());
                MessageBox.Show("Nothing in binary database, please add at least a face(Simply train the prototype with the Add Face Button).", "Triained faces load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        
        // Open File Image
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (flag)
            {
                flag = false;
                Application.Idle -= handler;
                grabber.Dispose();
            }
            try
            {
                OpenFileDialog of = new OpenFileDialog();
                if(of.ShowDialog()==DialogResult.OK)
                {
                    imgInput = new Image<Bgr, byte>(of.FileName);
                    imageBoxFrameGrabber.DisplayedImage = imgInput;
                    Application.Idle += new EventHandler(FrameImage);
                }
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void imageBoxFrameGrabber_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {

        }

        //Open Camera
        private void button1_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                grabber.Dispose();
            }
                //Initialize the capture device
                grabber = new Capture();
                grabber.QueryFrame();
            
            //Initialize the FrameGraber event
            if (!flag)
            {
                handler = new EventHandler(FrameGrabber);
                Application.Idle += handler;
            }
                flag = true;                
            //  button1.Enabled = false;
        }


        private void button2_Click(object sender, System.EventArgs e)
        {
            uploadFileIntoSystem();
        }

        private void button4_Click(Object sender, EventArgs e)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.SelectedPath = @"D:\Code Projects\C#\Friend projects\DATN_2019";
                    DialogResult result = folderDialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrEmpty(folderDialog.SelectedPath))
                    {
                        string[] temp = folderDialog.SelectedPath.Split('\\');
                        string personName = temp[temp.Length - 1];
                        //Console.WriteLine(personName);

                        foreach (string imagePath in Directory.GetFiles(folderDialog.SelectedPath))
                        {
                            this.imgInput = new Image<Bgr, byte>(imagePath);
                            //Console.WriteLine(imagePath);
                            this.uploadFileIntoSystem(personName);
    }
}
                }
            } catch (Exception)
            {
                Console.WriteLine("Stop doing something stupid");
            }
        }

        private void uploadFileIntoSystem(string label = null)
        {
            try
            { 
            //Trained face counter
                ContTrain = ContTrain + 1;

                //Get a gray frame from capture device

                //gray = grabber.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                
                imgInput = imgInput.Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                gray = imgInput.Convert<Gray, Byte>();

                //Face Detector
                //DetectHaarCascade(HaarCascade, Double, Int32, HAAR_DETECTION_TYPE, Size)
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                face,
                1.2,
                10,
                Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(20, 20));
                Console.WriteLine("Hello world");
                //Action for each element detected                
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    Console.WriteLine(f.rect);
                    TrainedFace = imgInput.Copy(f.rect).Convert<Gray, byte>();
                    
                    break;
                }
                Console.WriteLine("Hello world");

                //resize face detected image for force to compare the same size with the 
                //test image with cubic interpolation type method
                TrainedFace = TrainedFace.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                trainingImages.Add(TrainedFace);
                if (label.Equals(null))
                {
                    labels.Add(textBox1.Text);
                }
                else
                {
                    labels.Add(label);
                }
                Console.WriteLine("Hello world");

                //Show face added in gray scale
                imageBox1.Image = TrainedFace;

                //Write the number of triained faces in a file text for further load
                //Console.WriteLine(Application.StartupPath + "\\TrainedFaces\\TrainedLabels.txt");
                //Console.WriteLine(trainingImages.ToArray().Length);
                File.WriteAllText(Application.StartupPath + "\\TrainedFaces\\TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");
                Console.WriteLine("Hello world");

                //Console.WriteLine("File writen");
                //Write the labels of triained faces in a file text for further load
                for (int i = 1; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "\\TrainedFaces\\face" + i + ".bmp");
                    File.AppendAllText(Application.StartupPath + "\\TrainedFaces\\TrainedLabels.txt", labels.ToArray()[i - 1] + "%");
                }
                //Console.WriteLine("hello");

                MessageBox.Show(textBox1.Text + "´s face detected and added ", "Training OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception)
            {
                MessageBox.Show("Enable the face detection first", "Training Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

    void FrameImage(object sender, EventArgs e)
        {
            label3.Text = "0";
            NamePersons.Add("");

            //Get the curent frame from image
            currentFrame = imgInput.Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            gray = currentFrame.Convert<Gray, Byte>();

            //Face Detector
            MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
            face,
            1.2,
            10,
            Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
            new Size(20, 20));

            //Action for each element detected
            foreach (MCvAvgComp f in facesDetected[0])
            {
                t = t + 1;
                result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                //draw the face detected in the 0th (gray) channel with blue color
                currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                if (trainingImages.ToArray().Length != 0)
                {
                    //TermCriteria for face recognition with numbers of trained images like maxIteration
                    MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                    //Eigen face recognizer
                    EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                       trainingImages.ToArray(),
                       labels.ToArray(),
                       3500,
                       ref termCrit);

                    name = recognizer.Recognize(result);

                    //Draw the label for each face detected and recognized
                    currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                }

                NamePersons[t - 1] = name;
                NamePersons.Add("");


                //Set the number of faces detected on the scene
                label3.Text = facesDetected[0].Length.ToString();

                /*
                //Set the region of interest on the faces

                gray.ROI = f.rect;
                MCvAvgComp[][] eyesDetected = gray.DetectHaarCascade(
                   eye,
                   1.1,
                   10,
                   Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                   new Size(20, 20));
                gray.ROI = Rectangle.Empty;

                foreach (MCvAvgComp ey in eyesDetected[0])
                {
                    Rectangle eyeRect = ey.rect;
                    eyeRect.Offset(f.rect.X, f.rect.Y);
                    currentFrame.Draw(eyeRect, new Bgr(Color.Blue), 2);
                }
                 */

            }
            t = 0;

            //Names concatenation of persons recognized
            for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
            {
                names = names + NamePersons[nnn] + ", ";
            }
            //Show the faces procesed and recognized
            imageBoxFrameGrabber.Image = currentFrame;
            label4.Text = names;
            names = "";
            //Clear the list(vector) of names
            NamePersons.Clear();
        }

        void FrameGrabber(object sender, EventArgs e)
        {
            label3.Text = "0";
            //label4.Text = "";
            NamePersons.Add("");


            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                    //Convert it to Grayscale
                    gray = currentFrame.Convert<Gray, Byte>();

                    //Face Detector
                    MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                  face,
                  1.2,
                  10,
                  Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                  new Size(20, 20));

                    //Action for each element detected
                    foreach (MCvAvgComp f in facesDetected[0])
                    {
                        t = t + 1;
                        result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                        //draw the face detected in the 0th (gray) channel with blue color
                        currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                        if (trainingImages.ToArray().Length != 0)
                        {
                            //TermCriteria for face recognition with numbers of trained images like maxIteration
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                        //Eigen face recognizer
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                           trainingImages.ToArray(),
                           labels.ToArray(),
                           3500,
                           ref termCrit);

                        name = recognizer.Recognize(result);

                            //Draw the label for each face detected and recognized
                        currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                        }

                            NamePersons[t-1] = name;
                            NamePersons.Add("");


                        //Set the number of faces detected on the scene
                        label3.Text = facesDetected[0].Length.ToString();
                       
                        /*
                        //Set the region of interest on the faces
                        
                        gray.ROI = f.rect;
                        MCvAvgComp[][] eyesDetected = gray.DetectHaarCascade(
                           eye,
                           1.1,
                           10,
                           Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                           new Size(20, 20));
                        gray.ROI = Rectangle.Empty;

                        foreach (MCvAvgComp ey in eyesDetected[0])
                        {
                            Rectangle eyeRect = ey.rect;
                            eyeRect.Offset(f.rect.X, f.rect.Y);
                            currentFrame.Draw(eyeRect, new Bgr(Color.Blue), 2);
                        }
                         */

                    }
                        t = 0;

                        //Names concatenation of persons recognized
                    for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                    {
                        names = names + NamePersons[nnn] + ", ";
                    }
                    //Show the faces procesed and recognized
                    imageBoxFrameGrabber.Image = currentFrame;
                    label4.Text = names;
                    names = "";
                    //Clear the list(vector) of names
                    NamePersons.Clear();

                }

     /*   private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("");
        } */

    }
}