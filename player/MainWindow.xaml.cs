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
            this.previousButton.Click += previousButton_Click;
            this.nextButton.Click += nextButton_Click;
            this.fasterButton.Click += fasterButton_Click;
            this.slowerButton.Click += slowerButton_Click;
            //this.setPositionButton.Click += setPositionButton_Click;
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

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //ShowPosition();
            timeline.UpperValue = mediaElement.Position.TotalMilliseconds;

            // 계산 후 업데이트.
            this.heatmap.Source = mouseDecorder.GetHeatmap(mediaElement.Position);
        }

        private string recentOpenedPath = "%HomePath%";
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = recentOpenedPath;
                if (openFileDialog.ShowDialog() == true)
                {
                    string dictoryPath = openFileDialog.FileName;
                    string movieFile = openFileDialog.FileName;
                    string mtrFile = openFileDialog.FileName;

                    openFileDialog.InitialDirectory = recentOpenedPath;
                    this.mediaElement.Source = new Uri(@"C:\Users\JS\Documents\mouse-tracer\2021-03-26 15-01-17.mp4");
                    mouseDecorder = new MouseDecorder(@"C:\Users\JS\Documents\mouse-tracer\2021-03-26 15-01-17.mtr", this.mediaElement.NaturalVideoWidth, this.mediaElement.NaturalVideoHeight);
                    Console.WriteLine(File.ReadAllText(openFileDialog.FileName));
                }
            }
        }

        //private void ShowPosition()
        //{
        //    this.positionScrollBar.Value = this.mediaElement.Position.Ticks / TimeSpan.TicksPerMillisecond;
        //    this.positionTextBox.Text = this.mediaElement.Position.TotalSeconds.ToString("0.00");
        //}

        //private void setPositionButton_Click(object sender, RoutedEventArgs e)
        //{
        //    TimeSpan timespan = TimeSpan.FromSeconds(double.Parse(this.positionTextBox.Text));
        //    this.mediaElement.Position = timespan;

        //    ShowPosition();
        //}
        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.timeline.Minimum = 0;
            this.timeline.LowerValue = 0;
            this.timeline.UpperValue = 0;
            this.timeline.Maximum = this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            this.timeline.Visibility = Visibility.Visible;
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Play();

            SetButtnsEnabled(true);
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Pause();

            SetButtnsEnabled(false);
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Stop();

            SetButtnsEnabled(false);

            //ShowPosition();
        }

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Stop();
            this.mediaElement.Play();

            SetButtnsEnabled(true);
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Position -= TimeSpan.FromSeconds(10);

            //ShowPosition();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Position += TimeSpan.FromSeconds(10);

            //ShowPosition();
        }
        private void fasterButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.SpeedRatio *= 1.5d;
        }

        private void slowerButton_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement.SpeedRatio /= 1.5d;
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

        bool IsMovingPosition = false;
        private void positionScrollBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void positionScrollBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void positionScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            //Console.WriteLine(this.positionScrollBar.Value);
        }

        private void positionScrollBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMovingPosition = true;
            this.mediaElement.Pause();
            this.dispatcherTimer.Stop();
        }

        private void positionScrollBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMovingPosition)
            {
                //this.mediaElement.Position = TimeSpan.FromTicks((long)this.positionScrollBar.Value);
                //this.dispatcherTimer.Start();
                //this.mediaElement.Play();
                //IsMovingPosition = false;
            }
        }

        private void timeline_LowerValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.mediaElement.Position = TimeSpan.FromTicks((int)e.NewValue * TimeSpan.TicksPerMillisecond);
            //this.timeline.UpperValue = e.NewValue;
        }

        private void timeline_UpperThumbDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            this.timeline.UpperValueChanged += timeline_UpperThumbValueChanged;
        }

        private void timeline_UpperThumbDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            this.timeline.UpperValueChanged -= timeline_UpperThumbValueChanged;
        }

        private void timeline_UpperThumbValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.mediaElement.Position = TimeSpan.FromTicks((int)e.NewValue * TimeSpan.TicksPerMillisecond);
        }
    }
}
