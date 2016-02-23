using System.Collections.Generic;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace SoundVisualizer.Visualization
{
    public class Graph
    {
       
        private double _scale = 0.1;
        private List<Line> _lines = null;
        private int _width;
        private int _height;

        public double Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Graph()
        {
            _lines = new List<Line>();
        }



        protected void InputData(float[] values)
        {

            double dist = 1;
            double x1 = 0, y1 = 0;
            double x2 = 0, y2 = 0;
            bool flagFirst = true;
            

            for (int i = 0; i < values.Length; i++)
            {
 
                if (flagFirst)
                {
                    y1 = (_height - values[i] * _scale);
                    i++;
                    flagFirst = false;
                }

                x2 = x1 + dist;
                y2 = (_height - values[i] * _scale);

                

                var tempLine = new Line ();
                tempLine.Point1 = new Point(x1, y1);
                tempLine.Point2 = new Point(x2, y2);

                _lines.Add(tempLine);

                x1 = x2;
                y1 = y2;

            }
        }

        /// <summary>
        /// Paint.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public DrawingImage PaintGraph(int width, int height, float[] values)
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
