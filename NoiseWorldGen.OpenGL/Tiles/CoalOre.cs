using System.Runtime.CompilerServices;
using DotnetNoise;
using static DotnetNoise.FastNoise;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class CoalOre : FeatureTile, IOre, IInterpolation<CoalOre>, Tile.IsWalkable
{
    [ModuleInitializer]
    internal static void Init()
    {
        World.OnWorldCreated += w =>
            Noise = new(w.Seed ^ typeof(CoalOre).GetHashCode())
            {
                Frequency = .02f,
                UsedNoiseType = NoiseType.CubicFractal,
            };
        Game.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.Black });
            TileTemplates.Add<CoalOre>(new TileTemplate.Dynamic(static (w, p) =>
            {
                IOre ore = default!;
                Biomes.Biome.GenerateOre(p, ref ore!, static q => new CoalOre(q));
                return ore is CoalOre o ? o : new(1);
            }, texture, "Coal Ore"));
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
    public override string Name => $"Coal Ore ({Quantity})";

    public CoalOre(uint quantity)
        : base(Color.Black, Content.SpriteSheets.Ores.Instance.Texture, Content.SpriteSheets.Ores.CoalOre)
    {
        Quantity = quantity;
    }
}
