using System.Drawing;

namespace TagsCloudContainer.Extensions;

public static class RectangleExtensions
{
    public static bool IsIntersectsWithAny(this IEnumerable<Rectangle> rectangles, Rectangle rectangle)
    {
        return rectangles.Any(tag => tag.IntersectsWith(rectangle));
    }
}
