using System.Drawing;
using System.Windows.Forms;
using TagsCloudContainer.Extensions;

namespace TagsCloudContainer;

public class CircularCloudLayouter
{
    public readonly Point Center;
    public List<Tag> Tags { get; }
    public Color? BackgroundColor { get; private set; }
    public Color? TextColor { get; private set; }
    public string? FontName { get; private set; }
    public ISpiral Spiral { get; set; } = null!;

    public CircularCloudLayouter(Point center)
    {
        Center = center;
        Tags = [];
    }

    public CircularCloudLayouterVisualizer CreateView(int width, int height)
    {
        return new CircularCloudLayouterVisualizer(this, new Size(width, height));
    }

    public CircularCloudLayouter SetFontName(string font)
    {
        if (font.FontExists())
        {
            FontName = font;
        }

        return this;
    }

    public CircularCloudLayouter SetBackgroundColor(Color color)
    {
        BackgroundColor = color;
        return this;
    }
    
    public CircularCloudLayouter SetTextColor(Color color)
    {
        TextColor = color;
        return this;
    }

    public CircularCloudLayouter PutTags(Dictionary<string,int> words)
    {
        var sortedWords = words.OrderByDescending(pair => pair.Value);

        foreach (var word in sortedWords)
        {
            PutNextTag(word.Key, word.Value);
        }
        
        return this;
    }

    public Rectangle PutNextTag(string text, int count)
    {
        if (count < 1)
        {
            throw new ArgumentException(nameof(count));
        }
            
        var font = new Font(FontName ?? "Arial", count * 5 + 10);
        var rectangleSize = TextRenderer.MeasureText(text, font);
        
        Rectangle newRectangle;

        do
        {
            var location = Spiral.GetNextPoint();
            location.Offset(-rectangleSize.Width / 2, rectangleSize.Height / 2);
            newRectangle = new Rectangle(location, rectangleSize);
        }
        while (Tags.Select(tag => tag.Rectangle).IsIntersectsWithAny(newRectangle));

        newRectangle = ShiftRectangleToCenter(newRectangle);
        
        var tag = new Tag(text, font, newRectangle);
        Tags.Add(tag);
        return newRectangle;
    }

    public Size Size()
    {
        if (Tags.Count == 0)
            return System.Drawing.Size.Empty;

        var left = Tags.Select(tag => tag.Rectangle).Min(rectangle => rectangle.Left);
        var right = Tags.Select(tag => tag.Rectangle).Max(rectangle => rectangle.Right);
        var top = Tags.Select(tag => tag.Rectangle).Min(rectangle => rectangle.Top);
        var bottom = Tags.Select(tag => tag.Rectangle).Max(rectangle => rectangle.Bottom);

        return new Size(right - left, bottom - top);
    }

    private Rectangle ShiftRectangleToCenter(Rectangle rectangle)
    {
        var directionToCenter = GetDirectionToCenter(rectangle);
        while (directionToCenter != Point.Empty)
        {
            var nextRectangle = MoveRectangle(rectangle, directionToCenter);
            if (Tags.Select(tag => tag.Rectangle).IsIntersectsWithAny(nextRectangle))
                break;

            rectangle = nextRectangle;
            directionToCenter = GetDirectionToCenter(rectangle);
        }

        return rectangle;
    }

    private Point GetDirectionToCenter(Rectangle rectangle)
    {
        var rectangleCenter = new Point(
            rectangle.Left + rectangle.Width / 2,
            rectangle.Top - rectangle.Height / 2);

        return new Point(
            Math.Sign(Center.X - rectangleCenter.X),
            Math.Sign(Center.Y - rectangleCenter.Y)
        );
    }

    private static Rectangle MoveRectangle(Rectangle rectangle, Point direction)
    {
        return new Rectangle(
            new Point(rectangle.X + direction.X, rectangle.Y + direction.Y),
            rectangle.Size);
    }
}