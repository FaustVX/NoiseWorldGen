using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Forester : TickedFeatureTile
{

    [ModuleInitializer]
    internal static void Init()
            => World.OnWorldCreated += _ =>
            {
                var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
                texture.SetData(new Color[] { Color.LawnGreen });
                TileTemplates._tiles.Add(new TileTemplate.Dynamic((static (w, p) => new Forester(w, p)), texture, "Forester"));
            };
    public override string Name => $"Forester";
    public int Distance { get; } = 5;
    private Point? _lastOrePos;

    protected override void OnTick()
    {
        TickCount = 9;
        var rng = new Random();
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos.X, pos.Y) is null && World.GetSoilTileAt(pos.X, pos.Y) is IsFeaturePlacable)
            {
                _lastOrePos = pos;
                World.SetFeatureTileAt(pos.X, pos.Y, Tree.Value);
                break;
            }
            else
                _lastOrePos = null;
        }
    }

    public override void Draw(Rectangle tileRect, World world, Point pos)
    {
        base.Draw(tileRect, world, pos);
        if (_lastOrePos is {} lastOre)
            SpriteBatches.UI?.Draw(SpriteBatches.Pixel, tileRect.DrawAtWorldPos(pos, lastOre), Color.Lerp(Color.Black * 0, Color.Black * .75f, TickCount / 10f));
    }

    private Forester(World world, Point pos)
        : base(world, pos, Color.LawnGreen, default!)
    { }
}
