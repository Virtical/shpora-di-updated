using System.Drawing;

namespace TagsCloudContainer;

public class App
{
    public static string GetFileNameFromUser()
    {
        while (true)
        {
            Console.Write("Введите название файла с текстом: ");
            var fileName = Console.ReadLine();

            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                return fileName;
            }

            Console.WriteLine("Файл не найден. Попробуйте снова.");
        }
    }
    
    public static (int width, int height) GetImageDimensionsFromUser()
    {
        while (true)
        {
            Console.Write("Введите размер изображения (ширина высота): ");
            var input = Console.ReadLine()?.Split(' ');

            if (input?.Length == 2 &&
                int.TryParse(input[0], out int width) &&
                int.TryParse(input[1], out int height) &&
                width > 0 && height > 0)
            {
                return (width, height);
            }

            Console.WriteLine("Некорректный ввод. Убедитесь, что вы ввели два положительных целых числа.");
        }
    }
    
    public static string GetFontNameFromUser()
    {
        Console.Write("Введите название шрифта: ");
        return Console.ReadLine() ?? "Arial";
    }
    
    public static Color GetColorFromUser(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var colorInput = Console.ReadLine();

            if (Enum.TryParse(colorInput, true, out KnownColor knownColor))
            {
                return Color.FromKnownColor(knownColor);
            }

            Console.WriteLine("Некорректное название цвета. Попробуйте снова.");
        }
    }
}