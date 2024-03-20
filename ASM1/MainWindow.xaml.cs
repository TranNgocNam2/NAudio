using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ASM1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += OnTimerTick!;
            EnableControls(false);
            PopulateDevicesCombo();
        }

        private void PopulateDevicesCombo()
        {
            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                comboDevices.Items.Add(caps.ProductName);
            }
            comboDevices.SelectedIndex = 0;
        }

        private void EnableControls(bool isPlaying)
        {
            buttonPlay.IsEnabled = !isPlaying;
            buttonStop.IsEnabled = isPlaying;
            sliderPosition.IsEnabled = isPlaying;
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            if (reader != null)
            {
                textBlockPosition.Text = reader.CurrentTime.ToString();
                sliderPosition.Value = reader.CurrentTime.TotalSeconds;
            }
        }

        private WaveOut waveOut;
        private Mp3FileReader reader;

        private void PlayBtn(object sender, RoutedEventArgs e)
        {
            waveOut = new WaveOut();
            new WaveOut(WaveCallbackInfo.ExistingWindow(new WindowInteropHelper(this).Handle));
            waveOut.DeviceNumber = comboDevices.SelectedIndex;
            waveOut.NumberOfBuffers = 2;
            waveOut.DesiredLatency = 100;
            waveOut.Volume = (float)sliderVolume.Value;

            waveOut.PlaybackStopped += OnPlaybackStopped;
            reader = new Mp3FileReader("C:\\Users\\nam\\Desktop\\demo.mp3");
            textBlockDuration.Text = reader.TotalTime.ToString();
            textBlockPosition.Text = reader.CurrentTime.ToString();
            sliderPosition.Maximum = reader.TotalTime.TotalSeconds;
            sliderPosition.Value = 0;
            timer.Start();
            waveOut.Init(reader);
            waveOut.Play();
            EnableControls(true);
            //mediaElement.Source = new Uri("C:\\Users\\nam\\Desktop\\demo.mp3", UriKind.Relative);
            //mediaElement.LoadedBehavior = MediaState.Manual;
            //mediaElement.Volume = 0.5;
            //mediaElement.Play();
        }

        private void StopBtn(object sender, RoutedEventArgs e)
        {
           
            waveOut.Stop();
            //mediaElement.Stop();
        }
        
        void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            timer.Stop();
            sliderPosition.Value = 0;
            reader.Dispose();
            waveOut.Dispose();
            EnableControls(false);
            if (e.Exception != null)
            {
                MessageBox.Show(e.Exception.Message);
            }
        }
        private void SliderPositionOnDragCompleted(object sender, RoutedEventArgs e)
        {
            if (reader != null)
            {
                reader.CurrentTime = TimeSpan.FromSeconds(sliderPosition.Value);
            }
        }

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(waveOut != null)
            {
                waveOut.Volume = (float)sliderVolume.Value;
            }
        }

    }
}