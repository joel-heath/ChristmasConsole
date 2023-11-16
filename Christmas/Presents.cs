using Christmas.Audio;

namespace Christmas;
internal partial class Program
{
    private const double GOOD_PRESENT_PROBABILITY = 0.01;
    private const int GOOD_PRESENT_COOLDOWWN = 1; // number of seconds user cant move onto next present when getting a good present

    static List<Block> CreateBigPresent(ConsoleColor[] colors)
    {
        char symbol = '#';
        List<Block> present = [];
        Random random = new();

        // make sure lengths and widths are odd so there is a centre
        int width = 2 * random.Next(3, 7) + 1;
        int height = 2 * random.Next(3, 7) + 1;
        int xPos = (Console.WindowWidth - width) / 2;
        int yPos = (Console.WindowHeight - height) / 2;

        ConsoleColor primary = RandItem(colors);
        ConsoleColor secondary = RandItem(colors.Where(c => c != primary).ToList());

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

    class Present(string name, List<Block> item)
    {
        public string Name = name;
        public List<Block> Item = item;

        public Present ToPresent()
            => new(Name, Item.Select(b => new Block(b.Location, b.Color, b.Character)).ToList());
    }

    enum Behaviour { Neutral, Naughty, Nice };

    static void OpenPresents()
    {
        ConsoleWipe();
        AudioEngine.Instance.PlayLoopingMusic(@"Music\JoyToTheWorld.mp3");

        ConsoleColor[] presentColors = [ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Magenta];
        HashSet<Point> shakeVectors = [(1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1)];
        // define the point to move present to, then to move it back to center

        CachedSound[] boxSounds = [new(@"Sounds\BoxShake1.wav"), new(@"Sounds\BoxShake2.wav"), new(@"Sounds\BoxShake3.wav"), new(@"Sounds\BoxShake4.wav")];

        ConsoleColor d = ConsoleColor.DarkGray;
        ConsoleColor g = ConsoleColor.Gray;
        ConsoleColor r = ConsoleColor.Red;
        ConsoleColor b = ConsoleColor.DarkRed;
        ConsoleColor y = ConsoleColor.Yellow;
        ConsoleColor m = ConsoleColor.Magenta;
        ConsoleColor p = ConsoleColor.DarkMagenta;
        ConsoleColor w = ConsoleColor.White;
        ConsoleColor k = ConsoleColor.DarkGray;

        Present[] presents = [
            new Present("rock", [
                new((1, 0), g, '0'), new((2, 0), d, '@'), new((3, 0), d, '@'),
                new((0, 1), g, '#'), new((1, 1), g, '#'), new((2, 1), d, '0'), new((3, 1), g, '@'), new((4, 1), d, '#'),
                new((0, 2), g, '0'), new((1, 2), g, '@'), new((2, 2), g, '@'), new((3, 2), g, '#'), new((4, 2), g, '0'), new((5, 2), d, '0'),
                new((0, 3), d, '#'), new((1, 3), d, '0'), new((2, 3), d, '#'), new((3, 3), d, '@'), new((4, 3), g, '@'), new((5, 3), d, '0'),
                new((1, 4), g, '@'), new((2, 4), g, '0'), new((3, 4), d, '@'), new((4, 4), g, '#'), new((5, 4), d, '#'),
                new((2, 5), d, '@'), new((3, 5), g, '#'), new((4, 5), g, '#'), new((5, 5), d, '0')
            ]),

            new Present("sock", [
                new((3, 0), r, '0'), new((4, 0), r, '@'), new((5, 0), r, '@'),
                new((3, 1), y, '#'), new((4, 1), y, '#'), new((5, 1), y, '0'),
                new((3, 2), r, '0'), new((4, 2), r, '@'), new((5, 2), r, '@'),
                new((1, 3), y, '@'), new((2, 3), y, '@'), new((3, 3), y, '#'), new((4, 3), y, '0'), new((5, 3), y, '#'),
                new((0, 4), r, '#'), new((1, 4), r, '@'), new((2, 4), r, '0'), new((3, 4), r, '@'), new((4, 4), r, '#'), new((5, 4), r, '#'),
                new((0, 5), y, '@'), new((1, 5), y, '#'), new((2, 5), y, '#'), new((3, 5), y, '0'), new((4, 5), y, '#')
            ]),

            new Present("pair of swimming trunks", [
                new((0, 0), b, '#'), new((1, 0), b, '@'), new((2, 0), b, '0'), new((3, 0), b, '@'), new((4, 0), b, '#'), new((5, 0), b, '#'),
                new((0, 1), r, '@'), new((1, 1), r, '@'), new((2, 1), r, '#'), new((3, 1), r, '0'), new((4, 1), r, '#'), new((5, 1), r, '0'),
                new((0, 2), m, '0'), new((1, 2), m, '@'), new((4, 2), m, '@'), new((5, 2), m, '#'),
                new((0, 3), p, '@'), new((1, 3), p, '#'), new((4, 3), p, '#'), new((5, 3), p, '0')
            ]),

            new Present("box of chocolates", [
                new((0, 0), r, '#'), new((1, 0), r, '@'),                                           new((4, 0), r, '#'), new((5, 0), r, '#'),
                                     new((1, 1), r, '@'), new((2, 1), r, '#'), new((3, 1), r, '0'), new((4, 1), r, '#'),
                new((0, 2), p, '0'), new((1, 2), p, '@'), new((2, 2), p, '@'), new((3, 2), p, '#'), new((4, 2), p, '0'), new((5, 2), p, '0'),
                new((0, 3), p, '#'), new((1, 3), p, '0'), new((2, 3), p, '#'), new((3, 3), p, '@'), new((4, 3), p, '@'), new((5, 3), p, '0')
            ]),

            new Present("computer mouse", [
                                     new((1, 0), d, '#'), new((2, 0), d, '@'), new((3, 0), d, '#'),
                new((0, 1), d, '@'),                      new((2, 1), w, '0'),                      new((4, 1), d, '#'),
                new((0, 2), d, '0'), new((4, 2), d, '@'), new((0, 3), d, '@'), new((4, 3), d, '#'), new((0, 4), d, '0'), new((4, 4), d, '0'),
                                     new((1, 5), d, '0'), new((2, 5), d, '#'), new((3, 5), d, '@')
            ])
        ];

        Present[] goodPresents = [
            new Present("PlayStation 5", [
                new((0, 0), w, '#'), new((1, 0), k, '@'), new((2, 0), k, '0'), new((3, 0), k, '@'), new((4, 0), k, '#'), new((5, 0), w, '#'),
                new((0, 1), w, '@'), new((1, 1), w, '@'), new((2, 1), k, '#'), new((3, 1), k, '0'), new((4, 1), w, '#'), new((5, 1), w, '0'),
                                     new((1, 2), w, '0'), new((2, 2), k, '@'), new((3, 2), k, '@'), new((4, 2), w, '#'),
                                     new((1, 3), w, '#'), new((2, 3), k, '#'), new((3, 3), k, '0'), new((4, 3), w, '@'),
                new((0, 4), w, '0'), new((1, 4), w, '0'), new((2, 4), k, '@'), new((3, 4), k, '#'), new((4, 4), w, '@'), new((5, 4), w, '#'),
                new((0, 5), w, '@'), new((1, 5), w, '#'), new((2, 5), k, '#'), new((3, 5), k, '@'), new((4, 5), w, '#'), new((5, 5), w, '#')
            ])
        ];

        bool exit = false;
        while (!exit)
        {
            Console.Clear();

            Present present;
            Behaviour behaviour = Random.Shared.NextDouble() < GOOD_PRESENT_PROBABILITY ? Behaviour.Nice : Behaviour.Neutral;
            if (behaviour == Behaviour.Nice)
                present = RandItem(goodPresents).ToPresent();
            else
                present = RandItem(presents).ToPresent();
            List<Block> box = CreateBigPresent(presentColors);
            DrawMap(box);

            CachedSound sound = RandItem(boxSounds);

            for (int i = 0; i < Random.Shared.Next(4, 10); i++)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    exit = true;
                    break;
                }

                AudioEngine.Instance.PlaySound(sound);
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
            var time = DateTime.Now;
            if (behaviour == Behaviour.Nice)
                AudioEngine.Instance.PlaySound(new CachedSound(@"Sounds\Fanfare.wav"));
           
            while (!exit)
            {
                ConsoleKey keyPress = Console.ReadKey(true).Key;

                if (keyPress == ConsoleKey.Escape) { exit = true; break; }
                if (keyPress == ConsoleKey.Enter)
                {
                    if (behaviour == Behaviour.Nice && DateTime.Now - time < TimeSpan.FromSeconds(GOOD_PRESENT_COOLDOWWN)) continue;
                    break;
                }
            }
        }

        ConsoleWipe();
    }
}