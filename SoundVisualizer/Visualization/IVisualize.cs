using System.Windows.Media;

namespace SoundVisualizer.Visualization
{
    interface IVisualize
    {
        DrawingImage Drawing(int width, int height, float[] values);
    }
}
