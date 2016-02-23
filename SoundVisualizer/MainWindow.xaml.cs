using System;
using System.Windows;
using System.Windows.Media.Imaging;
using SoundVisualizer.Audio;
using SoundVisualizer.ProcessingAudio;
using SoundVisualizer.Recorder;
using SoundVisualizer.Visualization;


namespace SoundVisualizer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer _timer;
        private BitmapImage bitmapImage;
        private OpenALRecorder openOpenAlRecorder;
        private float[] _data;

        private int count = 0;
        public MainWindow()
        {
            
            InitializeComponent();

            _timer = new System.Windows.Threading.DispatcherTimer();

            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 90);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            openOpenAlRecorder = new OpenALRecorder();
            openOpenAlRecorder.SetOptions(openOpenAlRecorder.Devices[0], new AudioQuality(2,8,44100));
            openOpenAlRecorder.Recorded += openOpenAlRecorder_Recorded;
            openOpenAlRecorder.Start();

            _timer.Start();
           
        }
        
        void openOpenAlRecorder_Recorded(object sender, RecordedEventArgs e)
        {

            _data = new float[256];
            for (int i = 0; i < _data.Length; i ++)
            {
                _data[i] = e.Data[i*2];
            }
 
        }


        private void TimerTick(object sender, EventArgs e)
        {
            float[] fft = new float[_data.Length];
            FFT.Forward(_data, fft);
            image.Source = new Graph().PaintGraph((int)image.Width, (int)image.Height, _data);
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            openOpenAlRecorder.Stop();
            _timer.Stop();
        }

    }
}
