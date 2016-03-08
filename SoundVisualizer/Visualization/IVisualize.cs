using System.Windows.Media;

namespace SoundVisualizer.Visualization
{
    interface IVisualize
    {
        DrawingImage Drawing(int width, int height, double[] values);

        void SetColor(Color color);

        void SetLineThickness(double thickness);

        void Sensitivity(double sensitivity);


    }
}
