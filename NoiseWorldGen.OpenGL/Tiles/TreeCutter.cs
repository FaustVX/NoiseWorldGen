using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class TreeCutter : TickedFeatureTile
{

    [ModuleInitializer]
    internal static void Init()
        => Game1.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.Brown });
            TileTemplates._tiles.Add(new TileTemplate.Dynamic((static (w, p) => new TreeCutter(w, p)), texture, "Tree Cutter"));
        };
    public override string Name => $"Tree Cutter ({TreeStored} trees)";
    public int TreeStored { get; set; }
    public int Distance { get; } = 5;
    private Point? _lastOrePos;

    protected override void OnTick()
    {
        TickCount = 9;
        var rng = new Random();
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos) is Tree tree)
            {
                _lastOrePos = pos;
                tree.Mine(World, pos, this);
                TreeStored++;
                break;
            }
            else
                _lastOrePos = null;
        }
    }

    public override void Draw(Rectangle tileRect, World world, Point pos)
    {
        base.Draw(tileRect, world, pos);
        if (_lastOrePos is {} lastOre && SpriteBatches.UI is {} sb)
        {
            var destRectangle = tileRect.DrawAtWorldPos(pos, lastOre);
            var line = destRectangle.Center - tileRect.Center;
            var angleRad = MathF.Atan2(line.Y, line.X);
            var length = line.ToVector2().Length();
            sb.Draw(SpriteBatches.Pixel, tileRect.Center.ToVector2(), null, Lerp(Color.Red, TickCount), angleRad, Vector2.Zero, new Vector2(length, 1), default, 0);
            sb.Draw(SpriteBatches.Pixel, destRectangle, Lerp(Color.Black, TickCount));
            static Color Lerp(Color color, int tickCount)
                => Color.Lerp(color * 0, color * 75f, tickCount / 10f);
        }
    }

    private TreeCutter(World world, Point pos)
        : base(world, pos, Color.Brown, default!)
    { }
}
