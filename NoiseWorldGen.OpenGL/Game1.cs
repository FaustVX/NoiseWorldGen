using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoiseWorldGen.OpenGL.Content;
using NoiseWorldGen.OpenGL.Inputs;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace NoiseWorldGen.OpenGL;

public record class Point<T>(T X, T Y)
{
    public Point(T value)
        : this(value, value)
    { }
}
public record class Size<T>(T Width, T Height)
{
    public Size(T value)
        : this(value, value)
    { }
}

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly World _world;

    /// <summary>
    /// Top Left Tile
    /// </summary>
    public Point<int> TopLeftWorldPos { get; set; }

    /// <summary>
    /// Bottom Right Tile
    /// </summary>
    public Point<int> BottomRightWorldPos { get; set; }

    /// <summary>
    /// Size in pixels
    /// </summary>
    public int TileSize
    {
        get => tileSize;
        set
        {
            if (value < 1)
                return;
            tileSize = value;
            SetViewSize();
        }
    }
    private int tileSize = default!;

    /// <summary>
    /// Tile seen on screen
    /// </summary>
    public Size<int> ViewSize { get; set; }
    public bool ShowChunkBorders { get; set; }

    public Game1()
    {
        _world = new World(new Random().Next());
        _graphics = new GraphicsDeviceManager(this);
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        SpriteBatches.Game = new(GraphicsDevice);
        SpriteBatches.UI = new(GraphicsDevice);
        SpriteBatches.Pixel = new Texture2D(GraphicsDevice, 1, 1);
        SpriteBatches.Pixel.SetData(new[] { Color.White });

        TileSize = 16;
        SetViewSize();

        Components.Add(_world.Player);
        Components.Add(Keyboard.Instance);
    }


    [MemberNotNull(nameof(TopLeftWorldPos), nameof(BottomRightWorldPos))]
    [MemberNotNull(nameof(ViewSize))]
    private void SetViewSize()
    {
        ViewSize = new(GraphicsDevice.Viewport.Width / TileSize, GraphicsDevice.Viewport.Height / TileSize);
        SetBounds();
    }

    [MemberNotNull(nameof(TopLeftWorldPos), nameof(BottomRightWorldPos))]
    private void SetBounds()
    {
        TopLeftWorldPos = new((int)(_world.Player.Position.X - ViewSize.Width / 2), (int)(_world.Player.Position.Y - ViewSize.Height / 2));
        BottomRightWorldPos = new(TopLeftWorldPos.X + ViewSize.Width, TopLeftWorldPos.Y + ViewSize.Height);
    }

    protected override void LoadContent()
    {
        Textures.Ores = Content.Load<Texture2D>($@"{nameof(OpenGL.Content.SpriteSheets)}\{nameof(OpenGL.Content.SpriteSheets.Ores)}");
        Textures.Font = Content.Load<SpriteFont>($@"Fonts\{nameof(OpenGL.Content.Textures.Font)}");
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.Instance.IsDown(Keys.Escape))
            Exit();

        TileSize += Keyboard.XorFunc(Keyboard.Instance.IsClicked, Keys.Subtract, Keys.Add);

        ShowChunkBorders ^= (Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftAlt, Keys.RightAlt) && Keyboard.Instance.IsClicked(Keys.F1));

        base.Update(gameTime);

        SetBounds();
    }

    protected override bool BeginDraw()
    {
        SpriteBatches.Game.Begin();
        SpriteBatches.UI.Begin();
        return base.BeginDraw();
    }

    protected override void EndDraw()
    {
        SpriteBatches.Game.End();
        SpriteBatches.UI.End();
        base.EndDraw();
    }

    protected override void Draw(GameTime gameTime)
    {
        var tile = _world.GetTileAtPlayer();
        SpriteBatches.UI.Draw(tile.Texture ?? SpriteBatches.Pixel, new Vector2(0), tile.TextureRect, Color.White);
        SpriteBatches.UI.DrawString(Textures.Font, tile.GetType().Name, new Vector2(32, 0), Color.AliceBlue);
        if (tile is Tiles.IOre ore)
        {
            var size = Textures.Font.MeasureString(tile.GetType().Name);
            SpriteBatches.UI.DrawString(Textures.Font, $" ({ore.Quantity})", new Vector2(32 + size.X, 0), Color.White);
        }
        SpriteBatches.UI.DrawString(Textures.Font, _world.GetBiomeAtPlayer().GetType().Name, new Vector2(32, 25), Color.AliceBlue);

        for (int x = TopLeftWorldPos.X - 1; x <= BottomRightWorldPos.X; x++)
            for (int y = TopLeftWorldPos.Y - 1; y <= BottomRightWorldPos.Y; y++)
                DrawTile(x, y);

        base.Draw(gameTime);

        void DrawTile(int x, int y)
        {
            var tile = _world.GetTileAt(x, y);
            var x1 = (int)((x - _world.Player.Position.X + ViewSize.Width / 2f) * TileSize);
            var y1 = (int)((y - _world.Player.Position.Y + ViewSize.Height / 2f) * TileSize);

            SpriteBatches.Game.Draw(SpriteBatches.Pixel, new Rectangle(x1, y1, TileSize, TileSize), tile.Color);
            if (tile.Texture is { } texture)
                SpriteBatches.Game.Draw(texture, new Rectangle(x1, y1, TileSize, TileSize), tile.TextureRect, Color.White);

            if (ShowChunkBorders)
            {
                if (x % Chunck.Size == 0)
                    SpriteBatches.Game.Draw(SpriteBatches.Pixel, new Rectangle(x1, y1, 1, TileSize), Color.Black);
                if (y % Chunck.Size == 0)
                    SpriteBatches.Game.Draw(SpriteBatches.Pixel, new Rectangle(x1, y1, TileSize, 1), Color.Black);
            }
        }
    }
}
