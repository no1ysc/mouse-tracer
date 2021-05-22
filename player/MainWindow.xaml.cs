using Accord.Video.FFMPEG;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Threading;

namespace player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += Window_Loaded;
            this.mediaElement.MediaOpened += mediaElement_MediaOpened;
            this.playButton.Click += playButton_Click;
            this.pauseButton.Click += pauseButton_Click;
            this.stopButton.Click += stopButton_Click;
            this.fasterButton.Click += fasterButton_Click;
            this.slowerButton.Click += slowerButton_Click;
            this.loadButton.Click += loadButton_Click;
        }


        private DispatcherTimer dispatcherTimer;
        private MouseDecorder mouseDecorder;
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
            this.dispatcherTimer.Tick += dispatcherTimer_Tick;
            SetButtnsEnabled();
        }

        private string timespanToString(TimeSpan timespan)
        {
            if (timespan.Ticks < 0)
            {
                return string.Format("-{0:D2}:{1:D2}:{2:D2}.{3:D3}", Math.Abs(timespan.Hours), Math.Abs(timespan.Minutes), Math.Abs(timespan.Seconds), Math.Abs(timespan.Milliseconds));
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
            }
        }

        private bool IsFromDispatcher = false;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            IsFromDispatcher = true;
            timeline.Value = mediaElement.Position.TotalMilliseconds;
            IsFromDispatcher = false;
        }

        private string recentOpenedPath = "%HomePath%";
        private string mtrPath = "";
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = recentOpenedPath;
                if (openFileDialog.ShowDialog() == true)
                {
                    recentOpenedPath = openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf("\\"));
                    string movieFile = openFileDialog.FileName;

                    openFileDialog.InitialDirectory = recentOpenedPath;

                    mtrPath = movieFile.Substring(0, movieFile.LastIndexOf(".")) + ".mtr";
                    if (!File.Exists(mtrPath))
                    {
                        MessageBox.Show("Not exist mtr file.");
                        return;
                    }

                    this.mediaElement.Source = new Uri(movieFile);
                    mediaElement.Stop();
                }
            }
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlayMedia();
            PauseMedia();
            this.timeline.Minimum = 0;
            this.timeline.Value = 0;
            this.timeline.Maximum = this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            //this.timeline.Visibility = Visibility.Visible;

            this.startLine.Minimum = 0;
            this.startLine.Value = 0;
            this.startLine.Maximum = this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            //this.startLine.Visibility = Visibility.Visible;

            mouseDecorder = new MouseDecorder(mtrPath, this.mediaElement.NaturalVideoWidth, this.mediaElement.NaturalVideoHeight);
            heatmapIntensity.Value = mouseDecorder.HeatmapIntensity;
            heatmapRadius.Value = mouseDecorder.HeatmapRadius;
            //heatmapIntensity.IsEnabled = true;
            //heatmapRadius.IsEnabled = true;

            UpdateHeatmap();
        }

        private void UpdateHeatmap()
        {
            this.heatmap.Source = mouseDecorder.GetHeatmap(mediaElement.Position);            
        }

        enum MediaStatusEnum
        {
            Playing,
            Paused,
            Stopped,
            Initial,
        }

        private MediaStatusEnum MediaStatus = MediaStatusEnum.Initial;

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            PlayMedia();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            PauseMedia();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            StopMedia();
        }

        private double[] SpeedRatioTable = { 0.5d, 0.75d, 1.0d, 1.25d, 1.5d, 1.75d, 2.0d, 3.0d };
        private int SpeedRatioIndex = 2;
        private void fasterButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.SpeedRatio = SpeedRatioTable[SpeedRatioIndex == SpeedRatioTable.Length - 1 ? SpeedRatioTable.Length - 1 : ++SpeedRatioIndex];
            this.strPlaySpeed.Content = this.mediaElement.SpeedRatio;
        }

        private void slowerButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.SpeedRatio = SpeedRatioTable[SpeedRatioIndex == 0 ? 0 : --SpeedRatioIndex];
            this.strPlaySpeed.Content = this.mediaElement.SpeedRatio;
        }

        private void SetButtnsEnabled()
        {
            switch(MediaStatus)
            {
                case MediaStatusEnum.Playing:
                    this.playButton.IsEnabled = false;
                    this.playButton.Opacity = 0.5d;

                    this.pauseButton.IsEnabled = true;
                    this.pauseButton.Opacity = 1.0d;

                    this.slowerButton.IsEnabled = true;
                    this.slowerButton.Opacity = 1.0d;

                    this.fasterButton.IsEnabled = true;
                    this.fasterButton.Opacity = 1.0d;

                    this.stopButton.IsEnabled = true;
                    this.stopButton.Opacity = 1.0d;

                    this.btnExtract.IsEnabled = true;
                    this.timeline.Visibility = Visibility.Visible;
                    this.startLine.Visibility = Visibility.Visible;
                    this.heatmapIntensity.IsEnabled = true;
                    this.heatmapRadius.IsEnabled = true;
                    this.dispatcherTimer.IsEnabled = true;
                    break;
                case MediaStatusEnum.Paused:
                    this.playButton.IsEnabled = true;
                    this.playButton.Opacity = 1.0d;

                    this.pauseButton.IsEnabled = false;
                    this.pauseButton.Opacity = 0.5d;

                    this.slowerButton.IsEnabled = true;
                    this.slowerButton.Opacity = 1.0d;

                    this.fasterButton.IsEnabled = true;
                    this.fasterButton.Opacity = 1.0d;

                    this.stopButton.IsEnabled = true;
                    this.stopButton.Opacity = 1.0d;

                    this.btnExtract.IsEnabled = true;
                    this.timeline.Visibility = Visibility.Visible;
                    this.startLine.Visibility = Visibility.Visible;
                    this.heatmapIntensity.IsEnabled = true;
                    this.heatmapRadius.IsEnabled = true;
                    this.dispatcherTimer.IsEnabled = false;
                    break;
                case MediaStatusEnum.Stopped:
                    this.playButton.IsEnabled = true;
                    this.playButton.Opacity = 1.0d;

                    this.pauseButton.IsEnabled = false;
                    this.pauseButton.Opacity = 0.5d;

                    this.slowerButton.IsEnabled = true;
                    this.slowerButton.Opacity = 1.0d;

                    this.fasterButton.IsEnabled = true;
                    this.fasterButton.Opacity = 1.0d;

                    this.stopButton.IsEnabled = false;
                    this.stopButton.Opacity = 0.5d;

                    this.btnExtract.IsEnabled = false;
                    this.timeline.Visibility = Visibility.Hidden;
                    this.startLine.Visibility = Visibility.Hidden;
                    this.heatmapIntensity.IsEnabled = false;
                    this.heatmapRadius.IsEnabled = false;
                    this.dispatcherTimer.IsEnabled = false;
                    break;
                case MediaStatusEnum.Initial:
                    this.playButton.IsEnabled = false;
                    this.playButton.Opacity = 0.5d;

                    this.pauseButton.IsEnabled = false;
                    this.pauseButton.Opacity = 0.5d;

                    this.slowerButton.IsEnabled = false;
                    this.slowerButton.Opacity = 0.5d;

                    this.fasterButton.IsEnabled = false;
                    this.fasterButton.Opacity = 0.5d;

                    this.stopButton.IsEnabled = false;
                    this.stopButton.Opacity = 0.5d;

                    this.btnExtract.IsEnabled = false;
                    this.timeline.Visibility = Visibility.Hidden;
                    this.startLine.Visibility = Visibility.Hidden;
                    this.heatmapIntensity.IsEnabled = false;
                    this.heatmapRadius.IsEnabled = false;
                    this.dispatcherTimer.IsEnabled = false;
                    break;

            }
        }

        public string TimelineString
        {
            get;
            private set;
        }
        public static readonly DependencyProperty CompanyNameProperty =
            DependencyProperty.Register("TimelineString", typeof(string), typeof(MainWindow), new UIPropertyMetadata(string.Empty));

        private void SetMediaPosition(long offsetMilliseconds)
        {
            this.mediaElement.Position = TimeSpan.FromTicks(offsetMilliseconds * TimeSpan.TicksPerMillisecond);
        }

        private void SetMediaPosition(TimeSpan timeSpan)
        {
            this.mediaElement.Position = timeSpan;
        }


        private void PlayMedia()
        {
            MediaStatus = MediaStatusEnum.Playing;
            this.mediaElement.Play();

            SetButtnsEnabled();
        }

        private void StopMedia()
        {
            MediaStatus = MediaStatusEnum.Stopped;
            this.mediaElement.Stop();

            SetButtnsEnabled();

            startLine.Value = 0;
            timeline.Value = 0;
        }

        private void PauseMedia()
        {
            MediaStatus = MediaStatusEnum.Paused;
            this.mediaElement.Pause();

            SetButtnsEnabled();
        }

        private MediaStatusEnum prevMediaStatusUsingMouse;
        private void timeline_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            prevMediaStatusUsingMouse = MediaStatus;
            PauseMedia();
        }

        private void timeline_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (prevMediaStatusUsingMouse == MediaStatusEnum.Playing)
            {
                PlayMedia();
            }
        }

        private void timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan timeSpan = TimeSpan.FromTicks((int)e.NewValue * TimeSpan.TicksPerMillisecond);
            strCurrentPoint.Content = timespanToString(timeSpan);
            strDelta.Content = timespanToString(TimeSpan.FromTicks((int)(timeline.Value - startLine.Value) * TimeSpan.TicksPerMillisecond));
            // plotting heatmap
            if (e.NewValue < e.OldValue)
            {
                // if timeline value is decrase, re-plot heatmap using startLine time information.
                mouseDecorder.SetStartTime(TimeSpan.FromTicks((long)startLine.Value * TimeSpan.TicksPerMillisecond));
            }

            UpdateHeatmap();

            if (e.NewValue < startLine.Value)
            {
                timeline.Value = startLine.Value;
                return;
            }

            if (IsFromDispatcher)
            {
                return;
            }

            SetMediaPosition(timeSpan);
        }

        private void startLine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan timeSpan = TimeSpan.FromTicks((int)e.NewValue * TimeSpan.TicksPerMillisecond);
            strStartingPoint.Content = timespanToString(timeSpan);
            
            // Changing starting timespan of mouseDecorder
            mouseDecorder.SetStartTime(timeSpan);

            if (startLine.Value > timeline.Value)
            {
                timeline.Value = startLine.Value;
            }

            UpdateHeatmap();
        }

        private void startLine_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            prevMediaStatusUsingMouse = MediaStatus;
            PauseMedia();
        }

        private void startLine_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (prevMediaStatusUsingMouse == MediaStatusEnum.Playing)
            {
                PlayMedia();
            }
        }

        private MediaStatusEnum prevMediaStatusUsingKeyboard;
        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    {
                        if (MediaStatus != MediaStatusEnum.Playing)
                        {
                            PlayMedia();
                        }
                        else
                        {
                            PauseMedia();
                        }
                        e.Handled = true;
                    }
                    return;
                case Key.Right:
                    prevMediaStatusUsingKeyboard = MediaStatus;
                    PauseMedia();
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            startLine.Value += 10000;
                        }
                        else
                        {
                            startLine.Value += 1000;
                        }
                    } else
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            timeline.Value += 10000;
                        } else
                        {
                            timeline.Value += 1000;
                        }
                    }
                    if (prevMediaStatusUsingKeyboard == MediaStatusEnum.Playing)
                    {
                        PlayMedia();
                    }
                    e.Handled = true;
                    return;
                case Key.Left:
                    prevMediaStatusUsingKeyboard = MediaStatus;
                    PauseMedia();
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            startLine.Value -= 10000;
                        }
                        else
                        {
                            startLine.Value -= 1000;
                        }
                    }
                    else
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            timeline.Value -= 10000;
                        }
                        else
                        {
                            timeline.Value -= 1000;
                        }
                    }
                    if (prevMediaStatusUsingKeyboard == MediaStatusEnum.Playing)
                    {
                        PlayMedia();
                    }
                    e.Handled = true;
                    return;
                case Key.P:
                    Extract();
                    e.Handled = true;
                    return;
                case Key.S:
                    timeline.Value = startLine.Value;
                    e.Handled = true;
                    return;
                case Key.T:
                    startLine.Value = timeline.Value;
                    e.Handled = true;
                    return;
                case Key.X:
                    if (MediaStatus == MediaStatusEnum.Playing || MediaStatus == MediaStatusEnum.Paused)
                    {
                        mouseDecorder.TimeSpanOffset += TimeSpan.FromMilliseconds(200);
                        strTimespanOffset.Content = timespanToString(mouseDecorder.TimeSpanOffset);
                        UpdateHeatmap();
                    }
                    e.Handled = true;
                    return;
                case Key.Z:
                    if (MediaStatus == MediaStatusEnum.Playing || MediaStatus == MediaStatusEnum.Paused)
                    {
                        mouseDecorder.TimeSpanOffset -= TimeSpan.FromMilliseconds(200);
                        strTimespanOffset.Content = timespanToString(mouseDecorder.TimeSpanOffset);
                        UpdateHeatmap();
                    }
                    e.Handled = true;
                    return;
                default:
                    return;
            }
        }

        private void heatmapRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mouseDecorder != null)
            {
                mouseDecorder.HeatmapRadius = (int)e.NewValue;
                UpdateHeatmap();
            }
        }

        private void heatmapIntensity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mouseDecorder != null)
            {
                mouseDecorder.HeatmapIntensity = (byte)e.NewValue;
                UpdateHeatmap();
            }
        }

        private void Extract()
        {
            if (!btnExtract.IsEnabled)
            {
                return;
            }

            PauseMedia();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = recentOpenedPath;
            if (saveFileDialog.ShowDialog() == true)
            {
                mouseDecorder.savePartialMtr(saveFileDialog.FileName + ".mtr");
                SaveTimeInformation(saveFileDialog.FileName + ".ti");
                System.Drawing.Image heatmap = mouseDecorder.saveCurrentHeatmap(saveFileDialog.FileName + "-heatmap.png");
                ConvertUiElementToBitmap(this.mediaElement, saveFileDialog.FileName + "-player.png");
                ConvertUiElementToBitmap(this, saveFileDialog.FileName + "-capture.png");
                mergeImage(System.Drawing.Image.FromFile(saveFileDialog.FileName + "-player.png"), heatmap, 0.5).Save(saveFileDialog.FileName + ".png");

                MessageBox.Show("Saved.");
            }
        }

        public void SaveTimeInformation(string filePath)
        {
            StreamWriter streamWriter = new StreamWriter(filePath);
            
            streamWriter.WriteLine($"startTimeMs;{startLine.Value}");
            streamWriter.WriteLine($"endTimeMs;{timeline.Value}");
            streamWriter.WriteLine($"mouseOffset;{mouseDecorder.TimeSpanOffset}");

            streamWriter.Close();
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            Extract();
        }

        private Bitmap mergeImage(System.Drawing.Image bottom, System.Drawing.Image front, double opacity)
        {
            Bitmap bmp = new Bitmap(bottom.Width, bottom.Height); // Determining Width and Height of Source Image
            
            Graphics graphics = Graphics.FromImage(bmp);

            // bottom
            graphics.DrawImage(bottom, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bottom.Width, bottom.Height, GraphicsUnit.Pixel);


            ColorMatrix colormatrix = new ColorMatrix();

            colormatrix.Matrix33 = (float)opacity;

            ImageAttributes imgAttribute = new ImageAttributes();

            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            // front
            graphics.DrawImage(front, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, front.Width, front.Height, GraphicsUnit.Pixel, imgAttribute);

            graphics.Dispose();   // Releasing all resource used by graphics 

            return bmp;
        }

        //TODO: 좀있다가.
        public void saveMediaFrame(string filePath)
        {

            using (var vFReader = new VideoFileReader())
            {
                //vFReader.Open(mediaElement.Source.AbsolutePath);
                vFReader.Open(@"D:\07\07.mp4");
                vFReader. ReadVideoFrame((int)mediaElement.Position.TotalMilliseconds).Save(filePath);
                vFReader.Close();
            }
        }

        public static void ConvertUiElementToBitmap(UIElement elt, string path)
        {
            double h = elt.RenderSize.Height;
            double w = elt.RenderSize.Width;
            if (h > 0)
            {
                PresentationSource source = PresentationSource.FromVisual(elt);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)w, (int)h, 96, 96, PixelFormats.Default);

                VisualBrush sourceBrush = new VisualBrush(elt);
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                using (drawingContext)
                {
                    drawingContext.DrawRectangle(sourceBrush, null, new Rect(new System.Windows.Point(0, 0),
                          new System.Windows.Point(w, h)));
                }
                rtb.Render(drawingVisual);

                // return rtb;
                var encoder = new PngBitmapEncoder();
                var outputFrame = BitmapFrame.Create(rtb);
                encoder.Frames.Add(outputFrame);

                using (var file = File.OpenWrite(path))
                {
                    encoder.Save(file);
                }
            }
        }
    }
}
