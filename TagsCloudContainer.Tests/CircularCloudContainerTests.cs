using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudContainer.Tests;

[TestFixture]
public class CircularCloudContainerTests
{
    private CircularCloudLayouter circularCloudLayouter;
    
    [SetUp]
    public void Setup()
    {
        circularCloudLayouter = new CircularCloudLayouter(Point.Empty);
        var spiral = new ArchimedeanSpiral(Point.Empty);
        circularCloudLayouter.Spiral = spiral;
    }
    
    [Test]
    public void Constructor_SetCenterCorrectly_WhenInitialized()
    {
        var center = Point.Empty;
        var cloudCenter = circularCloudLayouter.Center;

        cloudCenter.Should().Be(center);
    }

    [Test]
    public void CloudSizeIsZero_WhenInitialized()
    {
        var actualSize = circularCloudLayouter.Size();

        actualSize.Should().Be(Size.Empty);
    }

    [Test]
    public void CloudSizeEqualsFirstTagSize_WhenPuttingFirstTag()
    {
        var font = new Font("Arial", 25);
        const string text = "text";
       
        SizeF expectsdRectangleSize;
        using (var bitmap = new Bitmap(1, 1))
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            expectsdRectangleSize = graphics.MeasureString(text, font);
        }

        circularCloudLayouter.PutNextTag(text, 3);
        var actualRectangleSize = circularCloudLayouter.Size();

        actualRectangleSize.Should().Be(expectsdRectangleSize.ToSize());
    }

    [Test]
    public void CloudSizeIsCloseToCircleShape_WhenPuttingManyTags()
    {
        for (var i = 0; i < 100; i++)
        {
            circularCloudLayouter.PutNextTag("test", 4);
        }

        var actualSize = circularCloudLayouter.Size();
        var aspectRatio = (double)actualSize.Width / actualSize.Height;

        aspectRatio.Should().BeInRange(0.5, 2.0);
    }

    [TestCase(0, TestName = "NoTags_WhenInitialized")]
    [TestCase(1, TestName = "SingleTag_WhenPuttingFirstTag")]
    [TestCase(10, TestName = "MultipleTags_WhenPuttingALotOfTags")]
    public void CloudContains(int rectangleCount)
    {
        for (var i = 0; i < rectangleCount; i++)
        {
            circularCloudLayouter.PutNextTag("test", 2);
        }

        circularCloudLayouter.Tags.Count.Should().Be(rectangleCount);
    }
    
    [Test]
    public void PutNextRectangle_ThrowException_WhenCountIsNotPositive()
    {
        var putIncorrectRectangle = () => circularCloudLayouter.PutNextTag("test", -5);

        putIncorrectRectangle.Should().Throw<ArgumentException>();
    }

    [Test]
    public void PutNextRectangle_PlacesFirstRectangleInCenter()
    {
        var center = Point.Empty;
        var firstRectangle = circularCloudLayouter.PutNextTag("text", 3);

        var rectangleCenter = new Point(
            firstRectangle.Left + firstRectangle.Width / 2,
            firstRectangle.Top - firstRectangle.Height / 2);

        rectangleCenter.Should().Be(center);
    }

    [Test]
    public void PutNextRectangle_CloudTagsIsNotIntersect_WhenPuttingALotOfTags()
    {
        circularCloudLayouter.PutNextTag("test", 3);
        circularCloudLayouter.PutNextTag("test", 2);
        circularCloudLayouter.PutNextTag("test", 1);

        var tags = circularCloudLayouter.Tags;
        for (var i = 0; i < tags.Count; i++)
        {
            var currentRectangle = tags[i].Rectangle;
            tags
                .Where((_, j) => j != i)
                .All(otherTag => !currentRectangle.IntersectsWith(otherTag.Rectangle))
                .Should().BeTrue();
        }
    }
}