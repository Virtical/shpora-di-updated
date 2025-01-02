using Aspose.Words;

namespace TagsCloudContainer;

public class WordProcessor
{
    private static readonly string excludedWordsPath = Path.Combine("..", "..", "..", "WordProcessing", "excluded_words.txt");
    private List<string> words;
    private HashSet<string> excludedWords = [];
    private bool enableDefaultExclude = true;

    public WordProcessor GetWordsForCloud(Func<string> wordsFunc)
    {
        var wordsPath = wordsFunc.Invoke();
        
        var extension = Path.GetExtension(wordsPath).ToLower();

        words = extension switch
        {
            ".txt" => GetWordsFromTxt(wordsPath),
            ".doc" or ".docx" => GetWordsFromDoc(wordsPath),
            _ => words
        };

        return this;
    }

    private static List<string> GetWordsFromDoc(string filePath)
    {
        var doc = new Document(filePath);
        var content = doc.GetText();
        return content.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLower())
            .Skip(1)
            .SkipLast(1)
            .ToList();
    }

    private static List<string> GetWordsFromTxt(string filePath)
    {
        return File.ReadAllLines(filePath)
            .Select(word => word.ToLower())
            .ToList();
    }
    
    public WordProcessor ExcludeWords(Func<string> excludedWordsFunc)
    {
        var wordsPath = excludedWordsFunc.Invoke();

        if (File.Exists(wordsPath))
        {
            excludedWords = File.ReadAllLines(wordsPath)
                .Select(word => word.ToLower())
                .ToHashSet();
        }

        return this;
    }

    public WordProcessor DisableDefaultExclude()
    {
        enableDefaultExclude = false;
        return this;
    }

    public Dictionary<string, int> ToDictionary()
    {
        if (enableDefaultExclude)
        {
            var newExcludedWords = File.ReadAllLines(excludedWordsPath)
                .Select(word => word.ToLower())
                .ToHashSet();
            
            excludedWords = excludedWords
                .Concat(newExcludedWords)
                .ToHashSet();
        }
        
        var wordCount = words
            .Where(word => !excludedWords.Contains(word))
            .GroupBy(word => word)
            .ToDictionary(group => group.Key, group => group.Count());
        
        return wordCount;
    }
}
