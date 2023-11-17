using Christmas.Audio;

namespace Christmas;
internal partial class Program
{
    struct Point(int x, int y)
    {
        public int X = x, Y = y;

        public static implicit operator Point((int, int) tuple) => new(tuple.Item1, tuple.Item2);

        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);

        public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Point a, Point b) => !(a == b);

        public override readonly bool Equals(object? obj) => obj is Point p && p.X.Equals(X) && p.Y.Equals(Y);
        public override readonly int GetHashCode() => HashCode.Combine(X, Y);
    }

    static T RandItem<T>(IEnumerable<T> items) => items.ElementAt(Random.Shared.Next(items.Count()));

    static void ConsoleWipe()
    {
        Console.SetCursorPosition(0, 0);
        for (int r = 0; r < Console.WindowHeight; r++)
        {
            if (r % 2 == 0) // forawrds
            {
                for (int c = 0; c < Console.WindowWidth; c++)
                {
                    Console.Write(' ');
                }
                try { Console.CursorTop++; }
                catch { }
            }
            else // backwards
            {
                for (int c = Console.WindowWidth - 1; c >= 0; c--)
                {
                    Console.Write(' ');
                    Console.CursorLeft = c;
                }
                try { Console.CursorTop++; }
                catch { }
            }
        }
    }

    static int Choose(string[] options, bool escapable = true)
    {
        int choice = 0;
        int indent = (Console.WindowWidth / 2) - (options.Sum(o => o.Length + 10) / 2);
        int xIndent = indent;
        int yIndent = Console.WindowHeight - (2 + 3);
        bool chosen = false;

        int textBoxLength = options.Max(o => o.Length);

        while (!chosen)
        {
            Console.CursorVisible = false;
            xIndent = indent;

            // write all options with current selected highlighted
            for (int i = 0; i < options.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(xIndent, yIndent);
                Console.WriteLine(new string('-', textBoxLength + 4));
                int centering = (textBoxLength - options[i].Length+1) / 2;
                Console.SetCursorPosition(xIndent, yIndent + 1);
                Console.Write("| " + new string(' ', centering));
                if (choice == i)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    //Console.ForegroundColor = options[i].Color;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                //Console.Write(options[i].Contents);
                Console.Write(options[i]);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;

                Console.CursorLeft = xIndent + textBoxLength + 2;
                Console.Write(" |");
                Console.SetCursorPosition(xIndent, yIndent + 2);
                Console.Write(new string('-', textBoxLength + 4));

                xIndent = Console.CursorLeft + 4;
            }

            bool keyPressed = false;
            while (!keyPressed)
            {
                keyPressed = true;
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.RightArrow:
                        if (choice < options.Length - 1)
                        {
                            //AudioPlaybackEngine.Instance.PlaySound(menuBleep);
                            choice++;
                        };
                        break;
                    case ConsoleKey.LeftArrow:
                        if (choice > 0)
                        {
                            choice--;
                        }
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter: chosen = true; break;
                    case ConsoleKey.Escape:
                        if (escapable)
                        {
                            choice = -1;
                            chosen = true;
                        }
                        break;
                    case ConsoleKey.F5: Console.Clear(); ViewTree(); break;
                    default:
                        keyPressed = false;
                        indent = (Console.WindowWidth / 2) - (options.Sum(o => o.Length + 10) / 2);
                        yIndent = Console.WindowHeight - (2 + 3);
                        break;
                }
            }
        }
        //Console.CursorVisible = true;
        return choice;
    }

    static void ViewTree()
    {
        ConsoleColor[] TreeColors = [ConsoleColor.Green, ConsoleColor.DarkGreen];
        ConsoleColor[] OrnamentColors = [ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Cyan];

        int rows = 18;
        int totalChars = rows * 2 - 1;
        int stars = 3;
        int spaces = (totalChars - stars) / 2;
        int centering = (Console.WindowWidth - totalChars - 2) / 2;

        lock (Console.ConsoleLock)
        {
            Console.SetCursorPosition((Console.WindowWidth - 1) / 2 - 1, 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('*');
            Console.CursorTop++;

            for (int row = 0; row < rows; row++)
            {
                Console.CursorLeft = centering + spaces;
                for (int i = 0; i < stars; i++)
                {
                    if (Random.Shared.NextDouble() > 0.8)
                    {
                        Console.ForegroundColor = RandItem(OrnamentColors);
                        Console.Write('o');
                    }
                    else
                    {
                        Console.ForegroundColor = RandItem(TreeColors);
                        Console.Write('*');
                    }
                }

                Console.CursorTop++;
                spaces--;
                stars += 2;
            }

            Console.CursorLeft = (Console.WindowWidth - 5) / 2;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("###"); Console.CursorTop++; Console.CursorLeft -= 5;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("# ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("### ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write('#');
            Console.CursorTop++; Console.CursorLeft -= 7;
            Console.Write("#######"); Console.CursorTop++; Console.CursorLeft -= 7;
            Console.Write("#######");
            /*
            catch (ArgumentOutOfRangeException)
            {
                // user pressed arrow key while tree was being drawn
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                return;
            }
            */
        }
        Console.ForegroundColor = ConsoleColor.White;
    }

    static void TickTimer(object? state)
    {
        ViewTree();
        //Thread.Sleep(800);
    }

    static void Main()
    {
        //Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();
        AudioEngine.Instance.PlayLoopingMusic(@"Music/InTheBleakMidwinter.mp3");
        Console.CursorVisible = false;
        string[] options = ["Look Outside", "Open A Present", "View Tree", "Pause"];
        string[] settings = ["Music Volume", "Sounds Volume"];
        Timer timer = new(new TimerCallback(TickTimer), null, 1000, 800);

        ViewTree();
        bool @continue = true;
        while (@continue)
        {
            switch (Choose(options, true))
            {
                case 0:
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    GoOutside();
                    AudioEngine.Instance.PlayLoopingMusic(@"Music/InTheBleakMidwinter.mp3");
                    ViewTree();
                    timer.Change(1000, 1000);
                    break;
                case 1:
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    OpenPresents();
                    AudioEngine.Instance.PlayLoopingMusic(@"Music/InTheBleakMidwinter.mp3");
                    ViewTree();
                    timer.Change(1000, 1000);
                    break;
                case 2:
                    Console.Clear();
                    ViewTree();
                    Console.ReadKey(true);
                    break;
                case 3:
                    AudioEngine.Instance.EnableLPF();
                    Console.Clear();
                    ViewTree();
                    var choice = Choose(settings);
                    if (choice > -1)
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        Console.Clear();
                        Console.Write("Enter new volume (0-1): ");
                        Console.CursorVisible = true;

                        float volume = -1;
                        while (volume < 0)
                        {
                            try
                            {
                                volume = Math.Max(Math.Min(float.Parse(Console.ReadLine() ?? "1"), 5), 0);
                            }
                            catch { }
                        }
                        Console.CursorVisible = false;

                        if (choice == 0)
                            AudioEngine.Instance.MusicVolume = volume;
                        else
                            AudioEngine.Instance.SoundsVolume = volume;
                    }
                    Console.Clear();
                    ViewTree();
                    timer.Change(1000, 1000);
                    AudioEngine.Instance.DisableLPF();
                    // https://github.com/naudio/NAudio/blob/master/NAudio.Extras/Equalizer.cs
                    break;
                case -1:
                    @continue = false;
                    break;
            }
        }
    }
}
