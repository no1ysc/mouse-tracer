using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace recorder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ~Form1()
        {
            if (vfw != null && vfw.IsOpen)
            {
                vfw.Flush();
                vfw.Close();
            }
        }

        private VideoFileWriter vfw;
        private MouseEventManager mem;
        Bitmap img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        Graphics g;
        string basePath = "";
        long tickCount = 0;

        private void prepareSavePath()
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\mouse-tracer\\";
            
            var di = new DirectoryInfo(basePath);
            if (!di.Exists)
            {
                di.Create();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tickCount++;

            #region writing screen
            g = Graphics.FromImage(img);
            g.CopyFromScreen(0, 0, 0, 0, img.Size);
            pictureBox1.Image = img;

            if (vfw != null && vfw.IsOpen)
            {
                vfw.WriteVideoFrame(img);
            }
            #endregion

            #region writing mouse

            #endregion

            if (tickCount % 1000 == 0)
            {
                vfw.Flush();
            }
        }

        private void btnRecording_Click(object sender, EventArgs e)
        {
            btnRecording.Enabled = false;
            btnStop.Enabled = true;

            prepareSavePath();
            string fileNameBase = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            #region writing screen
            vfw = new VideoFileWriter()
            {
                Width = Screen.PrimaryScreen.Bounds.Width,
                Height = Screen.PrimaryScreen.Bounds.Height,
                FrameRate = 30,
                BitRate = 1200 * 1000,
                VideoCodec = VideoCodec.Mpeg4
            };

            vfw.Open(basePath + fileNameBase + ".mp4");
            #endregion

            #region writing mouse
            mem = new MouseEventManager(basePath + fileNameBase + ".mtr");
            #endregion
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnRecording.Enabled = true;
            btnStop.Enabled = false;

            vfw.Close();
            mem.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnRecording.Enabled = true;
            btnStop.Enabled = false;

            timer1 = new Timer();
            timer1.Interval = 10;
            timer1.Tick += timer1_Tick;

            timer1.Start();
        }
    }
}
