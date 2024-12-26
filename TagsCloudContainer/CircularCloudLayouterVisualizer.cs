using System.Drawing;

namespace TagsCloudContainer;

public class CircularCloudLayouterVisualizer
{
    private CircularCloudLayouter layouter;
    private Size size;

    public CircularCloudLayouterVisualizer(CircularCloudLayouter layouter, Size bitmapSize)
    {
        this.layouter = layouter;
        size = bitmapSize;
    }
    
    public CircularCloudLayouter SaveImage(string filePath)
    {
        using var bitmap = new Bitmap(size.Width, size.Height);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.Clear(layouter.BackgroundColor ?? Color.White);

        var centerBitmap = new Point(size.Width / 2, size.Height / 2);
        var offsetBitmap = new Point(centerBitmap.X - layouter.Center.X, centerBitmap.Y - layouter.Center.Y);

        var brush = new SolidBrush(layouter.TextColor ?? Color.Black);

        foreach (var rectangle in layouter.Tags)
        {
            rectangle.Rectangle.Offset(offsetBitmap);
            graphics.DrawString(rectangle.Text, rectangle.Font, brush, rectangle.Rectangle);
        }

        bitmap.Save(filePath);
        
        return layouter;
    }
}