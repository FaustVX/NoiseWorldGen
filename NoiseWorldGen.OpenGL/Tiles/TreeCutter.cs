using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class TreeCutter : TickedFeatureTile, Tile.INetworkSupplier
{

    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.Brown });
            TileTemplates.Add<TreeCutter>(new TileTemplate.Dynamic((static (w, p) => new TreeCutter(w, p)), texture, "Tree Cutter"));
        };
    public override string Name => $"Tree Cutter ({TreeStored})";
    public TileStack TreeStored { get; }
    public int Distance { get; } = 5;
    public Networks.Network Network { get; set; } = default!;
    private Point? _lastOrePos;

    protected override void OnTick()
    {
        TickCount = 9;
        _lastOrePos = null;
        if (TreeStored.RemainingQuantity < 3)
            return;
        var rng = new Random();
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos) is Tree tree)
            {
                _lastOrePos = pos;
                tree.Mine(World, pos, this);
                TreeStored.Quantity += 3;
                break;
            }
        }
    }

    public override void Draw(Rectangle tileRect, World world, Point pos)
    {
        base.Draw(tileRect, world, pos);
        if (_lastOrePos is {} lastOre && SpriteBatches.UI is {} sb)
        {
            var destRectangle = tileRect.DrawAtWorldPos(pos, lastOre);
            sb.DrawLine(tileRect.Center, destRectangle.Center, Lerp(Color.Red, TickCount));
            sb.Draw(SpriteBatches.Pixel, destRectangle, Lerp(Color.Black, TickCount));
            static Color Lerp(Color color, int tickCount)
                => Color.Lerp(color * 0, color * 75f, tickCount / 10f);
        }
    }

    bool INetworkSupplier.CanSupply(TileTemplate tileTemplate)
        => TreeStored.Tile == tileTemplate && !TreeStored.IsEmpty;

    int INetworkSupplier.TrySupply(TileTemplate tileTemplate, int maxQuantity)
        => ((INetworkSupplier)this).CanSupply(tileTemplate) ? TreeStored.Request(maxQuantity) : 0;

    private TreeCutter(World world, Point pos)
        : base(world, pos, Color.Brown, default!)
    {
        TreeStored = new(TileTemplates.Get<Tree>());
    }
}
