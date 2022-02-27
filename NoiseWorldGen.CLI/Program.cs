using NoiseWorldGen.Core;

Console.Clear();
Console.CursorVisible = false;

var world = new World(Environment.TickCount, Console.WindowHeight - 3);
var width = Console.WindowWidth / 2 - 1;
var posX = 100_000;
var moveSize = 25;
DrawColumns(world, -width, width, width, posX);
do
{
    switch (Console.ReadKey().Key)
    {
        case ConsoleKey.RightArrow:
            posX += moveSize;
            if (OperatingSystem.IsWindows())
            {
                Console.MoveBufferArea(moveSize, 0, width * 2 - moveSize, Console.WindowHeight, 0, 0);
                DrawColumns(world, width - moveSize, width, width, posX);
            }
            else
                DrawColumns(world, -width, width, width, posX);
            break;
        case ConsoleKey.LeftArrow:
            posX -= moveSize;
            if (OperatingSystem.IsWindows())
            {
                Console.MoveBufferArea(0, 0, width * 2 - moveSize, Console.WindowHeight, moveSize, 0);
                DrawColumns(world, -width, -width + moveSize, width, posX);
            }
            else
                DrawColumns(world, -width, width, width, posX);
            break;
    }
} while (true);

static void DrawColumns(World world, int min, int max, int width, int posX)
{
    for (int x = min; x < max; x++)
        DrawColumn(world, x + posX, x + width);
}

static void DrawColumn(World world, int x, int screenX)
{
    for (int y = 0; y < world[x].Blocks.Length; y++)
        DrawBlock(world, x, y, screenX);
}

static void DrawBlock(World world, int x, int y, int screenX)
{
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive)
    (Console.ForegroundColor, Console.BackgroundColor, var symbol) = world[x, y] switch
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive)
    {
        Block.Bedrock => (ConsoleColor.Black, ConsoleColor.Black, '#'),
        Block.Stone => (ConsoleColor.Gray, ConsoleColor.Gray, '#'),
        Block.Water => (ConsoleColor.Blue, ConsoleColor.DarkBlue, '~'),
        Block.Air => (ConsoleColor.White, ConsoleColor.Black, ' '),
        Block.Dirt => (ConsoleColor.Green, ConsoleColor.Black, '#'),
        Block.Sand => (ConsoleColor.Yellow, ConsoleColor.DarkYellow, '"'),
    };
    Console.SetCursorPosition(screenX, world.Height - y);
    Console.Write(symbol);
}
