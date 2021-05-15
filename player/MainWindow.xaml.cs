using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            this.restartButton.Click += restartButton_Click;
            this.fasterButton.Click += fasterButton_Click;
            this.slowerButton.Click += slowerButton_Click;
            this.loadButton.Click += loadButton_Click;
        }


        private DispatcherTimer dispatcherTimer;
        private MouseDecorder mouseDecorder;
        private Graphics g;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
            this.dispatcherTimer.Tick += dispatcherTimer_Tick;
            this.mediaElement.Stop();
        }

        private string timespanToString(TimeSpan timespan)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
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
            this.timeline.Visibility = Visibility.Visible;

            this.startLine.Minimum = 0;
            this.startLine.Value = 0;
            this.startLine.Maximum = this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            this.startLine.Visibility = Visibility.Visible;

            mouseDecorder = new MouseDecorder(mtrPath, this.mediaElement.NaturalVideoWidth, this.mediaElement.NaturalVideoHeight);
            this.heatmap.Source = mouseDecorder.GetHeatmap(TimeSpan.Zero);
        }

        enum MediaStatusEnum
        {
            Playing,
            Paused,
            Stopped,
        }

        private MediaStatusEnum MediaStatus = MediaStatusEnum.Stopped;

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

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            StopMedia();
            PlayMedia();
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

        private void SetButtnsEnabled(bool isPlaying)
        {
            if (isPlaying)
            {
                this.playButton.IsEnabled = false;
                this.pauseButton.IsEnabled = true;
                this.playButton.Opacity = 0.5d;
                this.pauseButton.Opacity = 1.0d;
            }
            else
            {
                this.playButton.IsEnabled = true;
                this.pauseButton.IsEnabled = false;
                this.playButton.Opacity = 1.0d;
                this.pauseButton.Opacity = 0.5d;
            }
            this.dispatcherTimer.IsEnabled = isPlaying;
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

            SetButtnsEnabled(true);
        }

        private void StopMedia()
        {
            MediaStatus = MediaStatusEnum.Stopped;
            this.mediaElement.Stop();

            SetButtnsEnabled(false);

            startLine.Value = 0;
            timeline.Value = 0;
        }

        private void PauseMedia()
        {
            MediaStatus = MediaStatusEnum.Paused;
            this.mediaElement.Pause();

            SetButtnsEnabled(false);
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
            if (IsFromDispatcher)
            {
                return;
            }

            if (e.NewValue < startLine.Value)
            {
                timeline.Value = startLine.Value;
                return;
            }

            SetMediaPosition(timeSpan);

            // plotting heatmap
            if (e.NewValue < e.OldValue)
            {
                // if timeline value is decrase, re-plot heatmap using startLine time information.
                mouseDecorder.SetStartTime(TimeSpan.FromTicks((long)startLine.Value * TimeSpan.TicksPerMillisecond));
            }
            heatmap.Source = mouseDecorder.GetHeatmap(timeSpan);
        }

        private void startLine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan timeSpan = TimeSpan.FromTicks((int)e.NewValue * TimeSpan.TicksPerMillisecond);
            strStartingPoint.Content = timespanToString(timeSpan);
            
            // Changing starting timespan of mouseDecorder
            mouseDecorder.SetStartTime(timeSpan);
            
            timeline.Value = startLine.Value;
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
                    timeline.Value += 1000;
                    if (prevMediaStatusUsingKeyboard == MediaStatusEnum.Playing)
                    {
                        PlayMedia();
                    }
                    e.Handled = true;
                    return;
                case Key.Left:
                    prevMediaStatusUsingKeyboard = MediaStatus;
                    PauseMedia();
                    timeline.Value -= 1000;
                    if (prevMediaStatusUsingKeyboard == MediaStatusEnum.Playing)
                    {
                        PlayMedia();
                    }
                    e.Handled = true;
                    return;
                default:
                    return;
            }
        }
    }
}
