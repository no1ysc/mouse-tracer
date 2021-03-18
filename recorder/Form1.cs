using Accord.Audio;
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
        private MouseEventManager mem = new MouseEventManager();
        Bitmap lastFrame;
        AudioCaptureDevice audioDevice;
        ScreenCaptureStream screenStream;
        Graphics g;
        string basePath = "";
        long tickCount = 0;
        bool IsRecording = false;

        AudioSourceMixer audioMixer;

        private DateTime RecordingStartTime = DateTime.MinValue;
        private Object syncObj = new Object();


        public Form1()
        {
            InitializeComponent();

            InitScreenStream();

            //InitAudioDevices();

            //InitVideoFileWriter();
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
            vfw.VideoOptions["preset"] = "veryfast";
            vfw.VideoOptions["tune"] = "zerolatency";
            vfw.VideoOptions["x264opts"] = "no-mbtree:sliced-threads:sync-lookahead=0";

            vfw.AudioBitRate = VideoAudioBitRate;
            vfw.AudioCodec = AudioCodec.Aac;
            vfw.AudioLayout = audioMixer.NumberOfChannels == 1 ? AudioLayout.Mono : AudioLayout.Stereo;
            vfw.FrameSize = AudioDesiredFrameSize;
            vfw.SampleRate = audioMixer.SampleRate;
        }

        private void InitAudioDevices()
        {
            //var micDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture).ToArray();
            var audioDevices = new List<AudioCaptureDevice>();

            var outputDeviceInfo = new AudioDeviceCollection(AudioDeviceCategory.Output).Default;
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

            var micDeviceInfo = new AudioDeviceCollection(AudioDeviceCategory.Output).Default;
            if (micDeviceInfo != null)
            {
                var micDevice = new AudioCaptureDevice(micDeviceInfo.Guid);
                micDevice.Format = AudioSampleFormat;
                micDevice.SampleRate = AudioSampleRate;
                micDevice.DesiredFrameSize = AudioDesiredFrameSize;
                micDevice.AudioSourceError += AudioSourceError;
                micDevice.Start();

                audioDevices.Add(micDevice);
            }

            
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
            g = Graphics.FromImage(eventArgs.Frame);
            g.DrawIcon(Properties.Resources.mouse, mem.CurrentX, mem.CurrentY);
            lastFrame = eventArgs.Frame.Clone(new Rectangle(0, 0, eventArgs.Frame.Width, eventArgs.Frame.Height), eventArgs.Frame.PixelFormat);
            pictureBox1.Image = lastFrame;

            DateTime currentFrameTime = eventArgs.CaptureFinished;

            lock (syncObj) // Save the frame to the video file.
            {
                if (IsRecording)
                {
                    if (RecordingStartTime == DateTime.MinValue)
                        RecordingStartTime = DateTime.Now;

                    TimeSpan timestamp = currentFrameTime - RecordingStartTime;
                    if (timestamp > TimeSpan.Zero)
                        vfw.WriteVideoFrame(eventArgs.Frame, timestamp);
                }
            }

        }

        private void audioDevice_NewFrame(object sender, Accord.Audio.NewFrameEventArgs e)
        {
            lock (syncObj) // Save the frame to the video file.
            {
                if (IsRecording)
                {
                    vfw.WriteAudioFrame(e.Signal);
                }
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

        ~Form1()
        {
            if (vfw != null && vfw.IsOpen)
            {
                vfw.Flush();
                vfw.Close();
            }
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


            InitAudioDevices();

            InitVideoFileWriter();

            vfw.Open(basePath + fileNameBase + ".mp4");
            mem.Start(basePath + fileNameBase + ".mtr");

            IsRecording = true;
        }


        public void StopRecording()
        {
            if (!IsRecording)
                return;

            IsRecording = false;

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
    }
}
