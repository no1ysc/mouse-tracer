using Accord.Audio;
using Accord.Audio.Filters;
using Accord.DirectSound;
using Accord.Math;
using Accord.Video;
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
        private int AudioSampleRate { get; } = 44100;
        private SampleFormat AudioSampleFormat { get; } = SampleFormat.Format32BitIeeeFloat;
        private int AudioDesiredFrameSize { get; } = 10 * 4096;

        private int VideoFrameRate { get; } = 24;
        private int VideoBitRate { get; } = 1200 * 1000;
        private int VideoFrameSize { get; } = 44100;
        private int VideoAudioBitRate { get; } = 320 * 1000;

        private VideoFileWriter vfw;
        private MouseEventManager mem;
        ScreenCaptureStream screenStream;
        string basePath = "";
        bool IsRecording = false;

        AudioSourceMixer audioMixer;

        private DateTime RecordingStartTime = DateTime.MinValue;
        private Object syncObj = new Object();


        public Form1()
        {
            InitializeComponent();
            
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            StopRecording();
        }

        ~Form1()
        {
            StopRecording();
        }

        private void InitMouseEvent() 
        {
            mem = new MouseEventManager();
        }

        private void InitScreenStream()
        {
            screenStream = new ScreenCaptureStream(new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
            screenStream.FrameInterval = 1000 / VideoFrameRate;

            screenStream.NewFrame += ScreenStream_NewFrame;

            screenStream.Start();
        }


        private void InitVideoFileWriter()
        {
            vfw = new VideoFileWriter();
            vfw.BitRate = VideoBitRate;
            vfw.FrameRate = new Rational(1000, screenStream.FrameInterval); ;
            vfw.Width = Screen.PrimaryScreen.Bounds.Width;
            vfw.Height = Screen.PrimaryScreen.Bounds.Height;
            vfw.VideoCodec = VideoCodec.H264;
            vfw.VideoOptions["crf"] = "18"; // visually lossless
            vfw.VideoOptions["preset"] = "medium";
            vfw.VideoOptions["tune"] = "animation";
            vfw.VideoOptions["x264opts"] = "no-mbtree:sliced-threads:sync-lookahead=0";
            vfw.VideoOptions["flush_packets"] = "1";

            if (audioMixer != null)
            {
                vfw.AudioBitRate = VideoAudioBitRate;
                vfw.AudioCodec = AudioCodec.Aac;
                vfw.AudioLayout = audioMixer.NumberOfChannels == 1 ? AudioLayout.Mono : AudioLayout.Stereo;
                vfw.FrameSize = AudioDesiredFrameSize;
                vfw.SampleRate = audioMixer.SampleRate;
            }
        }

        private void InitAudioDevices()
        {
            var audioDevices = new List<AudioCaptureDevice>();

            //foreach(var outputDeviceInfo in new AudioDeviceCollection(AudioDeviceCategory.Output))
            //{
            //    var outputDevice = new AudioCaptureDevice(outputDeviceInfo.Guid);
            //    Console.WriteLine(outputDeviceInfo.Description);
            //    outputDevice.Format = AudioSampleFormat;
            //    outputDevice.SampleRate = AudioSampleRate;
            //    outputDevice.DesiredFrameSize = AudioDesiredFrameSize;
            //    outputDevice.AudioSourceError += AudioSourceError;
            //    outputDevice.Start();

            //    audioDevices.Add(outputDevice);
            //}

            AudioDeviceInfo outputDeviceInfo = new AudioDeviceCollection(AudioDeviceCategory.Capture).Default;
            if (outputDeviceInfo != null)
            {
                var outputDevice = new AudioCaptureDevice(outputDeviceInfo.Guid);
                outputDevice.Format = AudioSampleFormat;
                outputDevice.SampleRate = AudioSampleRate;
                outputDevice.DesiredFrameSize = AudioDesiredFrameSize;
                outputDevice.AudioSourceError += AudioSourceError;
                outputDevice.Start();

                audioDevices.Add(outputDevice);
            }

            //var micDeviceInfo = new AudioDeviceCollection(AudioDeviceCategory.Output).Default;
            //if (micDeviceInfo != null)
            //{
            //    var micDevice = new AudioCaptureDevice(micDeviceInfo.Guid);
            //    micDevice.Format = AudioSampleFormat;
            //    micDevice.SampleRate = AudioSampleRate;
            //    micDevice.DesiredFrameSize = AudioDesiredFrameSize;
            //    micDevice.AudioSourceError += AudioSourceError;
            //    micDevice.Start();

            //    audioDevices.Add(micDevice);
            //}


            if (audioDevices.Count > 0)
            {
                audioMixer = new AudioSourceMixer(audioDevices);
                audioMixer.AudioSourceError += AudioSourceError;
                audioMixer.NewFrame += audioDevice_NewFrame;
                audioMixer.Start();
            }
        }

        private void ScreenStream_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            //g = Graphics.FromImage(eventArgs.Frame);
            //g.DrawIcon(Properties.Resources.mouse, mem.CurrentX, mem.CurrentY);
            //lastFrame = null;
            //lastFrame = eventArgs.Frame.Clone(new Rectangle(0, 0, eventArgs.Frame.Width, eventArgs.Frame.Height), eventArgs.Frame.PixelFormat);
            //pictureBox1.Image = lastFrame;

            DateTime currentFrameTime = eventArgs.CaptureFinished;

            if (IsRecording)
            {
                if (RecordingStartTime == DateTime.MinValue)
                    RecordingStartTime = DateTime.Now;

                TimeSpan timestamp = currentFrameTime - RecordingStartTime;
                if (timestamp > TimeSpan.Zero)
                {
                    lock (syncObj) // Save the frame to the video file.
                    {
                        //vfw.WriteVideoFrame(eventArgs.Frame, timestamp);
                        vfw.WriteVideoFrame(eventArgs.Frame);
                    }
                }
            }
        }

        private void audioDevice_NewFrame(object sender, Accord.Audio.NewFrameEventArgs e)
        {
            if (IsRecording)
            {
                HighPassFilter hpf = new HighPassFilter(200, AudioSampleRate);
                LowPassFilter lpf = new LowPassFilter(20000, AudioSampleRate);
                Signal signal = lpf.Apply(hpf.Apply(e.Signal));

                lock (syncObj) // Save the frame to the video file.
                {
                    vfw.WriteAudioFrame(signal);
                }

                //float max = 0;
                //for(int i = 0; i < signal.Length / 10; i++)
                //{
                //    var sample = signal.GetSample(1, i);
                //    if (sample < 0)
                //    {
                //        sample = -sample;
                //    }
                //    if (sample > max)
                //    {
                //        max = sample;
                //    }
                //    progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = (int)(100f * max); });
                //}
            }
        }
        private void AudioSourceError(object sender, AudioSourceErrorEventArgs e)
        {
            StopRecording();

            IAudioSource source = sender as IAudioSource;
            source.SignalToStop();

            string msg = String.Format($"{source.Source} Error.");
            MessageBox.Show(msg, source.Source, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private void prepareSavePath()
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\mouse-tracer\\";

            var di = new DirectoryInfo(basePath);
            if (!di.Exists)
            {
                di.Create();
            }
        }

        public void StartRecording()
        {
            if (IsRecording)
            {
                return;
            }

            RecordingStartTime = DateTime.MinValue;

            prepareSavePath();
            string fileNameBase = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            InitScreenStream();
            InitAudioDevices();
            InitVideoFileWriter();
            InitMouseEvent();

            vfw.Open(basePath + fileNameBase + ".mp4");
            mem.Start(basePath + fileNameBase + ".mtr");

            IsRecording = true;
            RecIndicatorOn();
        }


        public void StopRecording()
        {
            if (!IsRecording)
            {
                return;
            }

            IsRecording = false;
            RecIndicatorOff();

            lock (syncObj)
            {
                if (vfw != null)
                {
                    vfw.Close();
                    vfw.Dispose();
                    vfw = null;
                }

                if (mem != null)
                {
                    mem.Stop();
                    mem = null;
                }

                if (screenStream != null)
                {
                    screenStream.Stop();
                    screenStream = null;
                }

                if (audioMixer != null)
                {
                    audioMixer.Stop();
                    foreach (IAudioSource source in audioMixer.Sources)
                    {
                        source.Stop();
                        source.Dispose();
                    }

                    audioMixer.Dispose();
                    audioMixer = null;
                }
            }
        }

        private Timer RecIndicator = null;
        private void RecIndicatorOn()
        {
            if (RecIndicator == null)
            {
                RecIndicator = new Timer();
                RecIndicator.Interval = 500;
                RecIndicator.Tick += RecIndicator_Tick;
            }
            
            RecIndicator.Start();
        }
        
        private void RecIndicatorOff()
        {
            RecIndicator.Stop();
            recSingal.ForeColor = Color.Gray;
        }
        private void RecIndicator_Tick(object sender, EventArgs e)
        {
            recSingal.ForeColor = recSingal.ForeColor == Color.Red ? Color.Gray : Color.Red;
        }

        private void btnRecording_Click(object sender, EventArgs e)
        {
            btnRecording.Enabled = false;
            btnStop.Enabled = true;

            StartRecording();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopRecording();

            btnRecording.Enabled = true;
            btnStop.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnRecording.Enabled = true;
            btnStop.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopRecording();
        }
    }
}
