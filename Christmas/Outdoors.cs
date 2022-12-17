using Christmas.Audio;
using System.Drawing;

namespace Christmas;
internal partial class Program
{
    class Snow
    {
        public Point Location;
        public ConsoleColor Color;
        public char Character;
        public int Velocity; // inverse (1/velocity)
        public int Timer;

        public Snow(Point location, ConsoleColor color, char character, int velocity)
        {
            this.Location = location;
            this.Color = color;
            this.Character = character;
            this.Velocity = velocity;
            this.Timer = 0;
        }
    }
    class Block
    {
        public Point Location;
        public ConsoleColor Color;
        public char Character;

        public Block(Point location, ConsoleColor color, char character)
        {
            this.Location = location;
            this.Color = color;
            this.Character = character;
        }

        public static explicit operator Block(Snow s) => new(s.Location, s.Color, s.Character);
    }

    static Snow MoveSnow(List<Block> map, Snow snow)
    {
        Point[] vectors = { (0, 1), (-1, 1), (1, 1) };

        foreach (Point vector in vectors)
        {
            Snow newSnow = new(snow.Location + vector, snow.Color, snow.Character, snow.Velocity);

            bool positionBlocked = false;
            foreach (Block block in map)
            {
                if (block.Location == newSnow.Location)
                    positionBlocked = true;
            }

            if (positionBlocked == true)
                continue;

            if (newSnow.Location.Y > Console.WindowHeight - 1 || newSnow.Location.X < 0 || newSnow.Location.X > Console.WindowWidth - 2)
                continue;

            return newSnow;
        }

        return snow;
    }

    static void DrawForegroundSnow(List<Block> map, List<Snow> snowflakes, List<Block> background, char[] symbols)
    {
        List<Snow> toRemove = new();

        for (int i = 0; i < snowflakes.Count; i++)
        {
            Snow snow = snowflakes[i];
            snow.Timer = ++snow.Timer % snow.Velocity;

            if (snow.Timer == 0)
            {
                Snow newSnow = MoveSnow(map, snow);
                if (newSnow.Location == snow.Location)
                {
                    map.Add((Block)snow);
                    toRemove.Add(snow);
                    continue;
                }

                var overwrite = background.Where(b => b.Location == snow.Location).DefaultIfEmpty(new Block(snow.Location, ConsoleColor.White, ' ')).First();
                Console.SetCursorPosition(overwrite.Location.X, overwrite.Location.Y);
                Console.ForegroundColor = overwrite.Color;
                Console.Write(overwrite.Character);

                Console.SetCursorPosition(newSnow.Location.X, newSnow.Location.Y);
                Console.ForegroundColor = newSnow.Color;
                newSnow.Character = RandItem(symbols.Where(s => s != snow.Character).ToArray());
                Console.Write(newSnow.Character);

                snowflakes[i] = newSnow;
            }
        }

        foreach (Snow snow in toRemove)
        {
            snowflakes.Remove(snow);
        }
    }

    static void DrawBackgroundSnow(List<Block> map, List<Snow> snowflakes, List<Block> foreground, char[] symbols)
    {
        List<int> toRemove = new();

        for (int i = 0; i < snowflakes.Count; i++)
        {
            Snow snow = snowflakes[i];
            snow.Timer = (snow.Timer + 1) % snow.Velocity;

            if (snow.Timer == 0)
            {
                Snow newSnow = MoveSnow(map, snow);
                if (newSnow.Location == snow.Location)
                {
                    map.Add((Block)snow);
                    toRemove.Add(i);
                    continue;
                }

                // overwrite old position if nothing in the way
                if (!foreground.Where(b => b.Location == snow.Location).Any())
                {
                    Console.SetCursorPosition(snow.Location.X, snow.Location.Y);
                    Console.Write(' ');
                }

                snowflakes[i] = newSnow;

                // dont overrite foregorund, go behind it
                if (foreground.Where(b => b.Location == newSnow.Location).Any()) continue;

                Console.SetCursorPosition(newSnow.Location.X, newSnow.Location.Y);
                Console.ForegroundColor = newSnow.Color;
                newSnow.Character = RandItem(symbols.Where(s => s != snow.Character).ToArray());
                Console.Write(newSnow.Character);
            }
        }

        foreach (int index in toRemove)
        {
            snowflakes.RemoveAt(index);
        }
    }

    static void DrawMap(List<Block> map)
    {
        for (int i = 0; i < map.Count; i++)
        {
            Console.SetCursorPosition(map[i].Location.X, map[i].Location.Y);
            Console.ForegroundColor = map[i].Color;
            Console.Write(map[i].Character);
        }
    }

    static List<Block> CreatePresent(ConsoleColor[] colors)
    {
        char symbol = '#';
        List<Block> present = new();
        Random random = new();

        // make sure lengths and widths are odd so there is a centre
        int width = 2 * random.Next(1, 5) + 1;
        int height = 2 * random.Next(1, 5) + 1;
        int xPos = random.Next(0, Console.WindowWidth - width);
        int yPos = random.Next(5, Console.WindowHeight - height);

        ConsoleColor primary = RandItem(colors);
        ConsoleColor secondary = RandItem(colors.Where(c => c != primary).ToArray());

        for (int y = yPos; y < yPos + height; y++)
        {
            for (int x = xPos; x < xPos + width; x++)
            {
                if (y - yPos == height / 2 || x - xPos == width / 2)
                {
                    present.Add(new((x, y), secondary, symbol));
                }
                else
                {
                    present.Add(new((x, y), primary, symbol));
                }
            }
        }

        return present;
    }

    static void GoOutside()
    {
        AudioEngine.Instance.PlayLoopingMusic(@"Music\YouSteppedDown.mp3");
        ConsoleWipe();
        Random random = new();

        ConsoleColor[] foregroundColors = new[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Magenta };
        ConsoleColor[] backgroundColors = new[] { ConsoleColor.DarkRed, ConsoleColor.DarkGreen, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.DarkYellow, ConsoleColor.DarkMagenta };

        List<Block> foreground = new();
        for (int i = 0; i < random.Next(4, 9); i++)
        {
            foreground.AddRange(CreatePresent(foregroundColors));
        }

        List<Block> background = new();
        for (int i = 0; i < random.Next(4, 9); i++)
        {
            background.AddRange(CreatePresent(backgroundColors));
        }

        List<Snow> foregroundSnow = new();
        List<Snow> backgroundSnow = new();

        DrawMap(background);
        DrawMap(foreground);

        //char[] snowflakes = new[] { '+', 'x', '*', '❄', '❅', '❆' };
        char[] snowflakes = new[] { '+', 'x', '*' };
        ConsoleColor[] foregroundSnowColors = new[] { ConsoleColor.White, ConsoleColor.Gray };

        while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape)
        {
            // add new snow
            if (foregroundSnow.Count < 8 && random.NextDouble() > 0.7)
                foregroundSnow.Add(new((random.Next(0, Console.WindowWidth), 0), RandItem(foregroundSnowColors), RandItem(snowflakes), random.Next(1, 4)));

            if (backgroundSnow.Count < 8 && random.NextDouble() > 0.7)
                backgroundSnow.Add(new((random.Next(0, Console.WindowWidth), 0), ConsoleColor.DarkGray, RandItem(snowflakes), random.Next(4, 10)));

            Thread.Sleep(50);

            DrawBackgroundSnow(background, backgroundSnow, foreground, snowflakes);
            DrawForegroundSnow(foreground, foregroundSnow, background, snowflakes);
        }

        ConsoleWipe();
    }
}