using Aspose.Words;
using TagsCloudContainer.ExcludedWordsProvider;

namespace TagsCloudContainer;

public class WordProcessor
{
    private readonly List<string> words = [];
    private readonly HashSet<string> excludedWords = [];
    private readonly Dictionary<string, IParser> parsers;
    private readonly ExcludedWordsSettings settings;
    
    public WordProcessor(Dictionary<string, IParser> handlers, ExcludedWordsSettings settings)
    {
        parsers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public WordProcessor GetWordsForCloud(string wordsPath)
    {
        if (!File.Exists(wordsPath)) return this;

        var extension = Path.GetExtension(wordsPath).ToLower();
        if (parsers.TryGetValue(extension, out var parser))
        {
            words.AddRange(parser.GetWords(wordsPath));
        }

        return this;
    }
    
    public WordProcessor ExcludeWords(IExcludedWordsProvider provider)
    {
        excludedWords.UnionWith(provider.GetExcludedWords());
        return this;
    }

    public WordProcessor DisableDefaultExclude()
    {
        settings.EnableDefaultExclude = false;
        return this;
    }

    public Dictionary<string, int> ToDictionary()
    {
        if (settings.EnableDefaultExclude)
        {
            var defaultExcludedWords = File.ReadAllLines(settings.DefaultExcludedWordsPath)
                .Select(word => word.ToLower())
                .ToHashSet();
            
            excludedWords.UnionWith(defaultExcludedWords);
        }
        
        return words
            .Where(word => !excludedWords.Contains(word))
            .GroupBy(word => word)
            .ToDictionary(group => group.Key, group => group.Count());
    }
}
