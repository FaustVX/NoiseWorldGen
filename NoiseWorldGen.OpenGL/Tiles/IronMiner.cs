using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class IronMiner : TickedFeatureTile
{

    [ModuleInitializer]
    internal static void Init()
            => World.OnWorldCreated += _ =>
            {
                var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
                texture.SetData(new Color[] { Color.SlateGray });
                TileTemplates._tiles.Add(new TileTemplate.Dynamic((static (w, p) => new IronMiner(w, p)), texture, "Iron Miner"));
            };
    public override string Name => $"Iron Miner ({TickCount} ticks, {IronStored} irons)";
    public int IronStored { get; set; }
    public int Distance { get; } = 5;

    protected override void OnTick()
    {
        TickCount = 9;
        var rng = new Random();
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos.X, pos.Y) is IronOre ore)
            {
                if (ore.Quantity > 1)
                    ore.Quantity--;
                else
                    World.SetFeatureTileAt(pos.X, pos.Y, null);
                IronStored++;
                break;
            }
        }
    }

    private IronMiner(World world, Point pos)
        : base(world, pos, Color.SlateGray, default!)
    { }
}
