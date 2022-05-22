using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class TreeCutter : TickedFeatureTile
{

    [ModuleInitializer]
    internal static void Init()
            => World.OnWorldCreated += _ =>
            {
                var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
                texture.SetData(new Color[] { Color.Brown });
                TileTemplates._tiles.Add(new TileTemplate.Dynamic((static (w, p) => new TreeCutter(w, p)), texture));
            };
    public override string Name => $"Tree Cutter ({TickCount} ticks, {TreeStored} trees)";
    public int TreeStored { get; set; }

    protected override void OnTick()
    {
        TickCount = 9;
        var rng = new Random();
        var pos = Extensions.GetRandomPointinCircle(10) + Pos;
        if (World.GetFeatureTileAt(pos.X, pos.Y) is Tree)
        {
            World.SetFeatureTileAt(pos.X, pos.Y, null);
            TreeStored++;
        }
    }

    private TreeCutter(World world, Point pos)
        : base(world, pos, Color.Brown, default!)
    { }
}
