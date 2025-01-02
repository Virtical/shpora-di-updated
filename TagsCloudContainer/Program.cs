using System.Drawing;
using Autofac;
using static TagsCloudContainer.App;

namespace TagsCloudContainer;

public static class Program
{
    public static void Main()
    {
        var imageDimensions = GetImageDimensionsFromUser("Введите размер изображения (по умолчанию W: 1000, H: 1000):");
        
        var container = ContainerConfig.Configure(imageDimensions.Center);
        
        var fileParser = new WordProcessor();
        var words = fileParser
            .GetWordsForCloud(() => GetFileNameFromUser("Введите название файла с текстом:"))
            .ExcludeWords(() => GetExcludedWordsFileNameFromUser("Введите название файла с исключёнными словами:"))
            .DisableDefaultExclude()
            .ToDictionary();

        using var scope = container.BeginLifetimeScope();
        
        var layouter = scope.Resolve<CircularCloudLayouter>();

        layouter
            .SetFontName(() => GetFontNameFromUser("Введите название шрифта (по умолчанию Arial):"))
            .SetBackgroundColor(() => GetColorsFromUser("Введите цвет фона (по умолчанию White):", Color.White))
            .SetTextColor(() => GetColorsFromUser("Введите цвет текста (по умолчанию Black):", Color.Black))
            .PutTags(words)
            .CreateView(imageDimensions.Width, imageDimensions.Height)
                .SaveImage("cloud.jpeg")
                .SaveImage("cloud.png")
                .SaveImage("cloud.bmp")
                .SaveImage("cloud.tiff");
    }
}