using System.Drawing;

namespace TagsCloudContainer.CloudLayouters;

public class CircularCloudLayouter
{
    public readonly Point Center;
    public List<Tag> Tags { get; }
    public (Color Primary, Color? Secondary)? BackgroundColor { get; private set; }
    public (Color Primary, Color? Secondary)? TextColor { get; private set; }
    private string? FontName { get; set; }
    private readonly RectangleArranger arranger;

    public CircularCloudLayouter(Point center, ISpiral spiral)
    {
        Center = center;
        Tags = [];
        arranger = new RectangleArranger(spiral);
    }

    public CircularCloudLayouterVisualizer CreateView(int width, int height)
    {
        return new CircularCloudLayouterVisualizer(this, new Size(width, height));
    }

    public CircularCloudLayouter SetFontName(string fontName)
    {
        FontName = fontName;
        return this;
    }

    public CircularCloudLayouter SetBackgroundColor((Color Primary, Color? Secondary) color)
    {
        BackgroundColor = color;
        return this;
    }
    
    public CircularCloudLayouter SetTextColor((Color Primary, Color? Secondary) color)
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

        var font = new Font(FontName ?? "Arial", count * 6 + 10);
        var rectangleSize = RectangleMeasurer.Measure(text, font);

        var rectangle = arranger.ArrangeRectangle(rectangleSize, Center);
        var tag = new Tag(text, font, rectangle);

        Tags.Add(tag);
        return rectangle;
    }
}