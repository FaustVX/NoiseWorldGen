using System;

namespace NoiseWorldGen.OpenGL;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using (var game = new Game1())
            game.Run();
    }
}
