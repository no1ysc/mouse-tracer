using Accord.Audio;
using Accord.DirectSound;
using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
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

            var audioDevices = new AudioDeviceCollection(AudioDeviceCategory.Output).ToArray();
            var micDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture).ToArray();
            
            if (audioDevices.Length > 0)
            {
                audioDevice = new AudioCaptureDevice(audioDevices[0].Guid);
                audioDevice.Format = SampleFormat.Format16Bit;
                audioDevice.SampleRate = 22000;
                audioDevice.DesiredFrameSize = 4096;
                audioDevice.NewFrame += audioDevice_NewFrame;
            }
        }

        private void audioDevice_NewFrame(object sender, NewFrameEventArgs e)
        {
            if (IsRecording)
            {
                vfw.WriteAudioFrame(e.Signal);
            }
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
        private MouseEventManager mem = new MouseEventManager();
        Bitmap img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        AudioCaptureDevice audioDevice;
        Graphics g;
        string basePath = "";
        long tickCount = 0;
        bool IsRecording = false;

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
            g.DrawIcon(Properties.Resources.mouse, mem.CurrentX, mem.CurrentY);
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
                if (vfw != null)
                {
                    vfw.Flush();
                }
            }
        }

        private void btnRecording_Click(object sender, EventArgs e)
        {
            btnRecording.Enabled = false;
            btnStop.Enabled = true;

            prepareSavePath();
            string fileNameBase = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            #region writing screen
            // 여기 셋팅 잘못하면 파일 오픈 안됨.
            vfw = new VideoFileWriter()
            {
                Width = Screen.PrimaryScreen.Bounds.Width,
                Height = Screen.PrimaryScreen.Bounds.Height,
                FrameRate = 30,
                BitRate = 1200 * 1000,
                FrameSize = 1024,
                VideoCodec = VideoCodec.Wmv1,
                //VideoCodec = VideoCodec.Mpeg4,
                AudioBitRate = 44100,
                SampleRate = audioDevice.SampleRate,
                AudioCodec = AudioCodec.WmaV1,
                //AudioCodec = AudioCodec.Aac,
                AudioLayout = AudioLayout.Mono,
            };

            vfw.Open(basePath + fileNameBase + ".wmv");
            #endregion

            #region writing mouse
            mem.Start(basePath + fileNameBase + ".mtr");
            #endregion

            audioDevice.Start();

            IsRecording = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            IsRecording = false;

            btnRecording.Enabled = true;
            btnStop.Enabled = false;

            vfw.Close();
            mem.Stop();
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
