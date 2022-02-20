using NoiseWorldGen.Core;

Console.Clear();
Console.CursorVisible = false;

var world = new World(Environment.TickCount, Console.WindowHeight - 3);
var width = Console.WindowWidth / 2 - 1;
var posX = width;
var redraw = true;
do
{
    if (redraw)
        for (int x = -width; x < width; x++)
            DrawColumn(world, x + posX, x + width);
    redraw = false;

    switch (Console.ReadKey().Key)
    {
        case ConsoleKey.RightArrow:
            posX++;
            redraw = true;
            break;
        case ConsoleKey.LeftArrow:
            posX--;
            redraw = true;
            break;
    }
} while (true);

static void DrawColumn(World world, int x, int screenX)
{
    for (int y = 0; y < world[x].Blocks.Length; y++)
        DrawBlock(world, x, y, screenX);
}

static void DrawBlock(World world, int x, int y, int screenX)
{
    (Console.ForegroundColor, var symbol) = world[x, y] switch
    {
        Block.Bedrock => (ConsoleColor.Black, '#'),
        Block.Stone => (ConsoleColor.Gray, '#'),
        Block.Water => (ConsoleColor.Blue, '~'),
        Block.Air => (ConsoleColor.White, ' '),
    };
    Console.SetCursorPosition(screenX, world.Height - y);
    Console.Write(symbol);
}
