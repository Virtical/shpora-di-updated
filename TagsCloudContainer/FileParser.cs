namespace TagsCloudContainer;

public class FileParser
{
    public static Dictionary<string, int> Parse(string wordsPath)
    {
        var excludedWordsPath = Path.Combine("..", "..", "..", "excluded_words.txt");

        var excludedWords = File.ReadAllLines(excludedWordsPath)
            .Select(word => word.ToLower())
            .ToList();
            
        var words = File.ReadAllLines(wordsPath)
            .Select(word => word.ToLower())
            .Where(word => !excludedWords.Contains(word))
            .ToList();
        
        var wordCount = words
            .GroupBy(word => word)
            .ToDictionary(group => group.Key, group => group.Count());

        return wordCount;
    }
}
