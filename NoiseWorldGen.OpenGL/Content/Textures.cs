using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.OpenGL.Content;

public static class Textures
{
    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += cm =>
        {
            Ores = cm.Load<Texture2D>($@"{nameof(SpriteSheets)}\{nameof(Ores)}");
            Font = cm.Load<SpriteFont>($@"Fonts\{nameof(Font)}");
        };
    public static Texture2D Ores { get; private set; } = default!;
    public static SpriteFont Font { get; private set; } = default!;
}