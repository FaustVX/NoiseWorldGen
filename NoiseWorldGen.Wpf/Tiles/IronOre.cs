using System.Runtime.CompilerServices;
using DotnetNoise;
using static DotnetNoise.FastNoise;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.Wpf.Tiles;

public sealed class IronOre : FeatureTile, IOre, IInterpolation<IronOre>, Tile.IsWalkable
{
    [ModuleInitializer]
    internal static void Init()
    {
        World.OnWorldCreated += w =>
            Noise = new(w.Seed ^ typeof(IronOre).GetHashCode())
            {
                Frequency = .02f,
                UsedNoiseType = NoiseType.CubicFractal,
            };
        MainWindowViewModel.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.Red });
            TileTemplates.Add<IronOre>(new TileTemplate.Dynamic(static (w, p) =>
            {
                IOre ore = default!;
                Biomes.Biome.GenerateOre(p, ref ore!, static q => new IronOre(q));
                return ore is IronOre o ? o : new(1);
            }, texture, "Iron Ore"));
        };
    }

    public override void Mine(World world, Point pos, Tile tile)
    {
        if (--Quantity <= 0)
            world.SetFeatureTileAt(pos, null);
    }

    public static uint MaxQuantity { get; } = 1000;
    public static FastNoise Noise { get; private set; } = default!;
    public static Interpolation Interpolation { get; } = new(0, (int)MaxQuantity)
    {
        { .3f, 0 },
    };
    public uint Quantity { get; set; }
    public override string Name => $"Iron Ore ({Quantity})";

    public override Rectangle TextureRect
        => Quantity switch
        {
            <= 100 => Content.SpriteSheets.Ores.IronOre0,
            <= 250 => Content.SpriteSheets.Ores.IronOre1,
            <= 500 => Content.SpriteSheets.Ores.IronOre2,
            <= 750 => Content.SpriteSheets.Ores.IronOre3,
            _ => Content.SpriteSheets.Ores.IronOre4,
        };

    public IronOre(uint quantity)
        : base(Color.Red, Content.SpriteSheets.Ores.Instance.Texture)
    {
        Quantity = quantity;
    }
}
