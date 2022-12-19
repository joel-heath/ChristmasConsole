using Christmas.Audio;
using System.Numerics;

namespace Christmas;
internal partial class Program
{
    static List<Block> CreateBigPresent(ConsoleColor[] colors)
    {
        char symbol = '#';
        List<Block> present = new();
        Random random = new();

        // make sure lengths and widths are odd so there is a centre
        int width = 2 * random.Next(3, 7) + 1;
        int height = 2 * random.Next(3, 7) + 1;
        int xPos = (Console.WindowWidth - width) / 2;
        int yPos = (Console.WindowHeight - height) / 2;

        ConsoleColor primary = RandItem(colors);
        ConsoleColor secondary = RandItem(colors.Where(c => c != primary).ToArray());

        for (int y = yPos; y < yPos + height; y++)
        {
            for (int x = xPos; x < xPos + width; x++)
            {
                if (y - yPos == height / 2 || x - xPos == width / 2)
                {
                    present.Add(new Block((x, y), secondary, symbol));
                }
                else
                {
                    present.Add(new Block((x, y), primary, symbol));
                }
            }
        }

        return present;
    }

    class Present
    {
        public string Name;
        public List<Block> Item;

        public Present(string name, List<Block> item)
        {
            this.Name = name;
            this.Item = item;
        }

        public Present ToPresent()
            => new(Name, Item.Select(b => new Block(b.Location, b.Color, b.Character)).ToList());
    }

    static void OpenPresents()
    {
        ConsoleWipe();
        AudioEngine.Instance.PlayLoopingMusic(@"Music\JoyToTheWorld.mp3");

        ConsoleColor[] presentColors = new[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Magenta };
        HashSet<Point> shakeVectors = new() { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1) };
        // define the point to move present to, then to move it back to center

        ConsoleColor d = ConsoleColor.DarkGray;
        ConsoleColor g = ConsoleColor.Gray;
        ConsoleColor r = ConsoleColor.Red;
        ConsoleColor b = ConsoleColor.DarkRed;
        ConsoleColor y = ConsoleColor.Yellow;
        ConsoleColor m = ConsoleColor.Magenta;
        ConsoleColor p = ConsoleColor.DarkMagenta;

        Present[] presents = new[]
        {
            new Present("Rock", new List<Block>() {
                new((1, 0), g, '0'), new((2, 0), d, '@'), new((3, 0), d, '@'),
                new((0, 1), g, '#'), new((1, 1), g, '#'), new((2, 1), d, '0'), new((3, 1), g, '@'), new((4, 1), d, '#'),
                new((0, 2), g, '0'), new((1, 2), g, '@'), new((2, 2), g, '@'), new((3, 2), g, '#'), new((4, 2), g, '0'), new((5, 2), d, '0'),
                new((0, 3), d, '#'), new((1, 3), d, '0'), new((2, 3), d, '#'), new((3, 3), d, '@'), new((4, 3), g, '@'), new((5, 3), d, '0'),
                new((1, 4), g, '@'), new((2, 4), g, '0'), new((3, 4), d, '@'), new((4, 4), g, '#'), new((5, 4), d, '#'),
                new((2, 5), d, '@'), new((3, 5), g, '#'), new((4, 5), g, '#'), new((5, 5), d, '0')}),

            new Present("Sock", new List<Block>() {
                new((3, 0), r, '0'), new((4, 0), r, '@'), new((5, 0), r, '@'),
                new((3, 1), y, '#'), new((4, 1), y, '#'), new((5, 1), y, '0'),
                new((3, 2), r, '0'), new((4, 2), r, '@'), new((5, 2), r, '@'),
                new((1, 3), y, '@'), new((2, 3), y, '@'), new((3, 3), y, '#'), new((4, 3), y, '0'), new((5, 3), y, '#'),
                new((0, 4), r, '#'), new((1, 4), r, '@'), new((2, 4), r, '0'), new((3, 4), r, '@'), new((4, 4), r, '#'), new((5, 4), r, '#'),
                new((0, 5), y, '@'), new((1, 5), y, '#'), new((2, 5), y, '#'), new((3, 5), y, '0'), new((4, 5), y, '#')}),

            new Present("pair of Swimming Trunks", new List<Block>() {
                new((0, 0), b, '#'), new((1, 0), b, '@'), new((2, 0), b, '0'), new((3, 0), b, '@'), new((4, 0), b, '#'), new((5, 0), b, '#'),
                new((0, 1), r, '@'), new((1, 1), r, '@'), new((2, 1), r, '#'), new((3, 1), r, '0'), new((4, 1), r, '#'), new((5, 1), r, '0'),
                new((0, 2), m, '0'), new((1, 2), m, '@'), new((4, 2), m, '@'), new((5, 2), m, '#'),
                new((0, 3), p, '@'), new((1, 3), p, '#'), new((4, 3), p, '#'), new((5, 3), p, '0')})
        };

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Present present = RandItem(presents).ToPresent();
            List<Block> box = CreateBigPresent(presentColors);
            DrawMap(box);


            for (int i = 0; i < Random.Shared.Next(4, 10); i++)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    exit = true;
                    break;
                }

                var vector = RandItem(shakeVectors);
                foreach (Block block in box)
                {
                    block.Location += vector;
                }
                Console.Clear();
                DrawMap(box);
                Thread.Sleep(50);
                foreach (Block block in box)
                {
                    block.Location -= vector;
                }
                Console.Clear();
                DrawMap(box);
            }

            if (exit) break;

            Console.Clear();

            int height = present.Item.Max(i => i.Location.Y);
            int length = present.Item.Max(i => i.Location.X);
            foreach (Block block in present.Item)
            {
                block.Location += ((Console.WindowWidth - length) / 2, (Console.WindowHeight - height) / 2);
            }

            DrawMap(present.Item);

            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.WriteLine($"You got a {present.Name}!");
            while (!exit)
            {
                ConsoleKey keyPress = Console.ReadKey(true).Key;

                if (keyPress == ConsoleKey.Escape) { exit = true; break; }
                if (keyPress == ConsoleKey.Enter) break;
            }
        }

        ConsoleWipe();
    }
}