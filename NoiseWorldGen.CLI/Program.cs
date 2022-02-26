using NoiseWorldGen.Core;

Console.Clear();
Console.CursorVisible = false;

var world = new World(Environment.TickCount, Console.WindowHeight - 3);
var width = Console.WindowWidth / 2 - 1;
var posX = width;
var moveSize = 25;
for (int x = -width; x < width; x++)
    DrawColumn(world, x + posX, x + width);
do
{
    switch (Console.ReadKey().Key)
    {
        case ConsoleKey.RightArrow:
            posX += moveSize;
            Console.MoveBufferArea(moveSize, 0, width * 2 - moveSize, Console.WindowHeight, 0, 0);
            for (int x = width - moveSize; x < width; x++)
                DrawColumn(world, x + posX, x + width);
            break;
        case ConsoleKey.LeftArrow:
            posX -= moveSize;
            Console.MoveBufferArea(0, 0, width * 2 - moveSize, Console.WindowHeight, moveSize, 0);
            for (int x = -width; x < -width + moveSize; x++)
                DrawColumn(world, x + posX, x + width);
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
    (Console.ForegroundColor, Console.BackgroundColor, var symbol) = world[x, y] switch
    {
        Block.Bedrock => (ConsoleColor.Black, ConsoleColor.Black, '#'),
        Block.Stone => (ConsoleColor.Gray, ConsoleColor.Gray, '#'),
        Block.Water => (ConsoleColor.Blue, ConsoleColor.DarkBlue, '~'),
        Block.Air => (ConsoleColor.White, ConsoleColor.Black, ' '),
    };
    Console.SetCursorPosition(screenX, world.Height - y);
    Console.Write(symbol);
}
