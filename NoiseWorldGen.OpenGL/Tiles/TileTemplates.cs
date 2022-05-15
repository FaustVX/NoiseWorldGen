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

public sealed class TileTemplate
{
    public Func<Tiles.Tile> Create { get; }
    public Texture2D Texture { get; }

    public TileTemplate(Func<Tile> create, Texture2D texture)
    {
        Create = create;
        Texture = texture;
    }
}