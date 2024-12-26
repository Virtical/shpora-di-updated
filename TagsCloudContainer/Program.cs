using System.Drawing;

namespace TagsCloudContainer;

public class Program
{
    public static void Main()
    {
        string fileName;

        while (true)
        {
            Console.Write("Введите название файла с текстом: ");
            fileName = Console.ReadLine()!;
            
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                break;
            }

            Console.WriteLine("Файл не найден. Попробуйте снова.");
        }
        
        int width, height;

        while (true)
        {
            Console.Write("Введите размер изображения (ширина высота): ");
            var input = Console.ReadLine()?.Split(' ');
            
            if (input?.Length == 2 &&
                int.TryParse(input[0], out width) &&
                int.TryParse(input[1], out height) &&
                width > 0 && height > 0)
            {
                break;
            }

            Console.WriteLine("Некорректный ввод. Убедитесь, что вы ввели два положительных целых числа.");
        }
        
        Console.Write("Введите название шрифта: ");
        var fontName = Console.ReadLine()!;
        
        Color backgroundColor;
        while (true)
        {
            Console.Write("Введите цвет фона (например, Red, Blue, Aqua): ");
            var bgColorInput = Console.ReadLine();

            if (Enum.TryParse(bgColorInput, true, out KnownColor bgKnownColor))
            {
                backgroundColor = Color.FromKnownColor(bgKnownColor);
                break;
            }

            Console.WriteLine("Некорректное название цвета. Попробуйте снова.");
        }

        Color textColor;
        while (true)
        {
            Console.Write("Введите цвет текста (например, Navy, Black, White): ");
            var textColorInput = Console.ReadLine();

            if (Enum.TryParse(textColorInput, true, out KnownColor textKnownColor))
            {
                textColor = Color.FromKnownColor(textKnownColor);
                break;
            }

            Console.WriteLine("Некорректное название цвета. Попробуйте снова.");
        }

        var words = FileParser.Parse(fileName);
        var center = new Point(width / 2, height / 2);
        
        var spiral = new ArchimedeanSpiral(center);

        var circularCloudLayouter = new CircularCloudLayouter(center, spiral);

        circularCloudLayouter
            .SetFontName(fontName)
            .SetBackgroundColor(backgroundColor)
            .SetTextColor(textColor)
            .PutTags(words)
            .CreateView(width, height).SaveImage("text_as_rectangle.png");
    }
}