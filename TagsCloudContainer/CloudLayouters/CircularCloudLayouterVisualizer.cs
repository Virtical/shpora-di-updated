using System.Drawing;
using System.Drawing.Drawing2D;

namespace TagsCloudContainer;

public class CircularCloudLayouterVisualizer
{
    private CircularCloudLayouter layouter;
    private Size size;
    private (Color Primary, Color? Secondary)? backgroudColor;
    private (Color Primary, Color? Secondary)? textColor;

    public CircularCloudLayouterVisualizer(CircularCloudLayouter layouter, Size bitmapSize)
    {
        this.layouter = layouter;
        size = bitmapSize;
        backgroudColor = layouter.BackgroundColor?.Invoke();
        textColor = layouter.TextColor?.Invoke();
    }
    
    public CircularCloudLayouterVisualizer SaveImage(string filePath)
    {
        using var bitmap = new Bitmap(size.Width, size.Height);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.Clear(backgroudColor?.Primary ?? Color.White);

        if (backgroudColor?.Secondary != null)
        {
            var bounds = new Rectangle(0, 0, size.Width, size.Height);
            
            using var gradientBrush = new LinearGradientBrush(
                bounds,
                backgroudColor?.Primary ?? Color.White,
                backgroudColor?.Secondary ?? Color.White,
                LinearGradientMode.Horizontal);

            graphics.FillRectangle(gradientBrush, bounds);
        }

        var centerBitmap = new Point(size.Width / 2, size.Height / 2);
        var offsetBitmap = new Point(centerBitmap.X - layouter.Center.X, centerBitmap.Y - layouter.Center.Y);

        foreach (var rectangle in layouter.Tags)
        {
            rectangle.Rectangle.Offset(offsetBitmap);

            if (textColor?.Secondary != null)
            {
                var gradientBrush = new LinearGradientBrush(
                    rectangle.Rectangle,
                    textColor?.Primary ?? Color.Blue,
                    textColor?.Secondary ?? Color.Red,
                    LinearGradientMode.Horizontal);
                
                graphics.DrawString(rectangle.Text, rectangle.Font, gradientBrush, rectangle.Rectangle);
            }
            else
            {
                var brush = new SolidBrush(textColor?.Primary ?? Color.Black);
                graphics.DrawString(rectangle.Text, rectangle.Font, brush, rectangle.Rectangle);
            }
        }

        bitmap.Save(filePath);
        
        return this;
    }
}