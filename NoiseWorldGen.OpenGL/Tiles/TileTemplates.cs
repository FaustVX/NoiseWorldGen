using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL;

public static class TileTemplates
{
    internal static readonly List<TileTemplate> _tiles = new();

    public static IReadOnlyList<TileTemplate> Tiles => _tiles;
    public static int CurrentIndex { get; set; }
    public static TileTemplate CurrentTemplate
        => Tiles[CurrentIndex];
}

public abstract class TileTemplate
{
    public sealed class Static : TileTemplate
    {
        public Func<Tiles.Tile> Create { get; }

        public Static(Func<Tile> create, Texture2D texture)
            : base(texture)
        {
            Create = create;
        }
    }
    public sealed class Dynamic : TileTemplate
    {
        public Func<World, Point, Tiles.Tile> Create { get; }

        public Dynamic(Func<World, Point, Tile> create, Texture2D texture)
            : base(texture)
        {
            Create = create;
        }
    }
    public Texture2D Texture { get; }

    private TileTemplate(Texture2D texture)
    {
        Texture = texture;
    }
}