using System.Drawing;
using Autofac;

namespace TagsCloudContainer;

public class Program
{
    public static void Main()
    {
        var fileName = App.GetFileNameFromUser();
        
        var (width, height) = App.GetImageDimensionsFromUser();
        var center = new Point(width / 2, height / 2);
        
        var builder = new ContainerBuilder();
        
        builder.RegisterType<ArchimedeanSpiral>()
            .As<ISpiral>()
            .WithParameter("center", center);
        
        builder.RegisterType<CircularCloudLayouter>()
            .AsSelf()
            .WithParameter("center", center)
            .PropertiesAutowired();
        
        var container = builder.Build();
        
        var fontName = App.GetFontNameFromUser();
        var backgroundColor = App.GetColorFromUser("Введите цвет фона (например, Red, Blue, Aqua): ");
        var textColor = App.GetColorFromUser("Введите цвет текста (например, Navy, Black, White): ");
        
        var words = FileParser.Parse(fileName);

        using var scope = container.BeginLifetimeScope();
        
        var layouter = scope.Resolve<CircularCloudLayouter>();

        layouter
            .SetFontName(fontName)
            .SetBackgroundColor(backgroundColor)
            .SetTextColor(textColor)
            .PutTags(words)
            .CreateView(width, height).SaveImage("text_as_rectangle.png");
    }
}