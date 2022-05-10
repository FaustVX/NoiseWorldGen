using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.OpenGL.Content.SpriteSheets;

public sealed class Ores : SpriteSheet
{
    public static readonly Ores Instance = new();
    public static readonly Rectangle CoalOre = Instance[nameof(CoalOre)];
    public static readonly Rectangle IronOre0 = Instance[nameof(IronOre0)];
    public static readonly Rectangle IronOre1 = Instance[nameof(IronOre1)];
    public static readonly Rectangle IronOre2 = Instance[nameof(IronOre2)];
    public static readonly Rectangle IronOre3 = Instance[nameof(IronOre3)];
    public static readonly Rectangle IronOre4 = Instance[nameof(IronOre4)];
    public Ores()
        : base(Textures.Ores, spriteSize: 32)
    {
        this[nameof(CoalOre)] = BySpriteSize(0, 0);
        this[nameof(IronOre0)] = BySpriteSize(2, 4);
        this[nameof(IronOre1)] = BySpriteSize(2, 3);
        this[nameof(IronOre2)] = BySpriteSize(2, 2);
        this[nameof(IronOre3)] = BySpriteSize(2, 1);
        this[nameof(IronOre4)] = BySpriteSize(2, 0);
    }
}