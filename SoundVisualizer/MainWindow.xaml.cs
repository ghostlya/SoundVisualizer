using System;
using System.Linq;
using System.Windows;
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
        private readonly System.Windows.Threading.DispatcherTimer _timer;
        private OpenALRecorder _openOpenAlRecorder;
        private double[] _data;
        public MainWindow()
        {
            
            InitializeComponent();

            _timer = new System.Windows.Threading.DispatcherTimer();

            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 70);

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            _openOpenAlRecorder = new OpenALRecorder();
            _openOpenAlRecorder.SetOptions(_openOpenAlRecorder.Devices[1], new AudioQuality(2,8,44100));
            _openOpenAlRecorder.Recorded += openOpenAlRecorder_Recorded;
            _openOpenAlRecorder.Start();

            _timer.Start();
           
        }
        
        void openOpenAlRecorder_Recorded(object sender, RecordedEventArgs e)
        {
            _data = new double[512];

            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = e.Data[i];

            }

           // _dataProcessing.AudioData = temp;

        }


        private void TimerTick(object sender, EventArgs e)
        {
            image.Source = new Graph().Drawing((int)image.Width, (int)image.Height, _data);
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            _openOpenAlRecorder.Stop();
            _timer.Stop();
        }

    }
}
