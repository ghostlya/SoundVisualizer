using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using SoundVisualizer.ProcessingAudio;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace SoundVisualizer.Visualization
{
    public class Graph : IVisualize
    {

        private double _scaleY = 0.1;
        private double _scaleX = 1;
        private List<Line> _lines = null;
        private int _width;
        private int _height;
        private double _minY;
        private double _maxY;

        public Graph()
        {
            _lines = new List<Line>();
        }

        private void InputData(float[] values)
        {
            Complex[] a = new Complex[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                a[i] = values[i];
            }
            Complex[] fft = FFT.Fft(a);

            double[] x = new double[fft.Length / 4];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = fft[i].Real;
            }


            _maxY = x.Max();
            _minY = x.Min();

            CreateLines(x);
        }

        private void CalculatуScale(int count)
        {
            _scaleY = _height / (_maxY - _minY) * 5;
            _scaleX = _width / (count + 1);
        }


        protected void CreateLines(double[] values)
        {
            CalculatуScale(values.Length);

            double x1 = 0, y1 = 0;
            double x2 = 0, y2 = 0;

            double tempVal = 0;
            var tempLine = new Line();

            for (int i = 1; i < values.Length; i++)
            {

                tempVal = (values[i] * _scaleY);

                if (tempVal < 0)
                {
                    tempVal = 0;
                }

                if (tempVal > _height)
                {
                    tempVal = _height;
                }

                x2 = x1 + 2;
                y2 = tempVal;

                tempLine = new Line();

                tempLine.Point1 = new Point(x1 * _scaleX, y1);
                tempLine.Point2 = new Point(x2 * _scaleX, y2);

                _lines.Add(tempLine);

                x1 = x2;
                y1 = y2;

            }

            tempLine = new Line();

            tempLine.Point1 = new Point(x1 * _scaleX, y1);
            tempLine.Point2 = new Point(x2 * _scaleX, 0);
            _lines.Add(tempLine);

        }


        public DrawingImage Drawing(int width, int height, float[] values)
        {
            _width = width;
            _height = height;

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext dc = drawingVisual.RenderOpen();

            InputData(values);

            for (int i = 0; i < _lines.Count; i++)
            {
                dc.DrawLine(_lines[i].Pen, _lines[i].Point1, _lines[i].Point2);
            }

            dc.Close();

            return new DrawingImage(drawingVisual.Drawing);
        }


    }

    class Line
    {
        private Pen _pen = new Pen(Brushes.Green, 1);
        private Point _point1;
        private Point __point2;

        public Pen Pen
        {
            get { return _pen; }
            set { _pen = value; }
        }

        public Point Point1
        {
            get { return _point1; }
            set { _point1 = value; }
        }

        public Point Point2
        {
            get { return __point2; }
            set { __point2 = value; }
        }
    }
}
