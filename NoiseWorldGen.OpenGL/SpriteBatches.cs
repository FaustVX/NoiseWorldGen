using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.OpenGL;

public static class SpriteBatches
{
    public static Texture2D Pixel { get; internal set; } = default!;
    public static SpriteBatch Game { get; internal set; } = default!;
    public static SpriteBatch? UI { get; internal set; } = default!;
}