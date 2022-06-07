using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.Content.SpriteSheets;

public abstract class SpriteSheet
{
    protected readonly int SpriteSize;
    public static readonly string Whole = nameof(Whole);
    private readonly Dictionary<string, Rectangle> Sections;

    public Texture2D Texture { get; }

    protected SpriteSheet(Texture2D texture, int spriteSize)
    {
        Texture = texture;
        SpriteSize = spriteSize;
        Sections = new();
        this[Whole] = texture.Bounds;
    }

    public Rectangle this[string section]
    {
        get => Sections[section];
        protected set => Sections[section] = value;
    }

    protected Rectangle BySpriteSize(int x, int y, int width = 1, int height = 1)
        => new(x * SpriteSize, y * SpriteSize, width * SpriteSize, height * SpriteSize);
}