using Autofac;
using TagsCloudContainer.CloudLayouters;
using TagsCloudContainer.ExcludedWordsProvider;
using TagsCloudContainer.Settings;
using TagsCloudContainer.WordProcessing.ExcludedWordsProvider;
using static TagsCloudContainer.App;

namespace TagsCloudContainer;

public static class Program
{
    public static void Main()
    {
        using var scope = ContainerConfig.Configure().BeginLifetimeScope();
        
        var layouter = scope.Resolve<CircularCloudLayouter>();
        var imageSettings = scope.Resolve<ImageSettings>();
        var fontSettings = scope.Resolve<FontSettings>();
        var colorSettings = scope.Resolve<ColorSettings>();
        var filesSettings = scope.Resolve<FilesSettings>();

        var wordProcessor = scope.Resolve<WordProcessorFactory>().Create();
        var fileExcludedWordsProvider = scope.Resolve<ExcludedWordsProviderFactory>().Create();
        
        var words = wordProcessor
            .GetWordsForCloud(filesSettings.Words)
            .ExcludeWords(fileExcludedWordsProvider)
            .DisableDefaultExclude()
            .ToDictionary();;

        layouter
            .SetFontName(fontSettings.FontName)
            .SetBackgroundColor(colorSettings.BackgroundColor)
            .SetTextColor(colorSettings.TextColor)
            .PutTags(words)
            .CreateView(imageSettings.Width, imageSettings.Height)
                .SaveImage("cloud.jpeg")
                .SaveImage("cloud.png")
                .SaveImage("cloud.bmp")
                .SaveImage("cloud.tiff");
    }
}