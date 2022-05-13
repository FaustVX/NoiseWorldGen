using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoiseWorldGen.OpenGL.Content;
using NoiseWorldGen.OpenGL.Inputs;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

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
    public World World { get; }

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

    private bool _isFullScreen = false;
    private Size<int> _defaultWindowSize;
    public bool IsFullScreen
    {
        get => _isFullScreen;
        set
        {
            if (value == _isFullScreen)
                return;
            _isFullScreen = value;
            if (IsFullScreen)
            {
                Window.IsBorderless = true;
                (_, _, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight) = GraphicsDevice.Adapter.CurrentDisplayMode.TitleSafeArea;
                _graphics.ApplyChanges();
            }
            else
            {
                (_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight) = _defaultWindowSize;
                Window.IsBorderless = false;
                _graphics.ApplyChanges();
            }
        }
    }

    public Game1()
    {
        World = new World(new Random().Next());
        _graphics = new GraphicsDeviceManager(this);
        _graphics.ApplyChanges();
        _defaultWindowSize = new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (s, e) => SetViewSize();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        SpriteBatches.Game = new(GraphicsDevice);
        SpriteBatches.UI = new(GraphicsDevice);
        SpriteBatches.Pixel = new Texture2D(GraphicsDevice, 1, 1);
        SpriteBatches.Pixel.SetData(new[] { Color.White });

        TileSize = 16;
        SetViewSize();

        Components.Add(World.Player);
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
        TopLeftWorldPos = new((int)(World.Player.Position.X - ViewSize.Width / 2), (int)(World.Player.Position.Y - ViewSize.Height / 2));
        BottomRightWorldPos = new(TopLeftWorldPos.X + ViewSize.Width, TopLeftWorldPos.Y + ViewSize.Height);
    }

    public (int x, int y) WorldToScreen(float x, float y)
    {
        var x1 = (int)((x - World.Player.Position.X + ViewSize.Width / 2f) * TileSize);
        var y1 = (int)((y - World.Player.Position.Y + ViewSize.Height / 2f) * TileSize);
        return (x1, y1);
    }

    public (float x, float y) ScreenToWorld(int x, int y)
    {
        var x1 = ((float)x / TileSize) - ViewSize.Width / 2f + World.Player.Position.X;
        var y1 = ((float)y / TileSize) - ViewSize.Height / 2f + World.Player.Position.Y;
        if (x1 < 0)
            x1--;
        if (y1 < 0)
            y1--;
        return (x1, y1);
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

        ShowChunkBorders ^= (Keyboard.Instance.IsClicked(Keys.F1) && Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftAlt, Keys.RightAlt));
        IsFullScreen ^= (Keyboard.Instance.IsClicked(Keys.Enter) && Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftControl, Keys.RightControl));

        for (int x = TopLeftWorldPos.X - 9; x <= BottomRightWorldPos.X + 9; x += 8)
            for (int y = TopLeftWorldPos.Y - 9; y <= BottomRightWorldPos.Y + 9; y += 8)
                World.GetChunkAtPos(x, y, out _, out _).Initialize();

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
        var mouseState = Mouse.GetState();
        var cursorPos = ScreenToWorld(mouseState.Position.X, mouseState.Position.Y);
        if (World.GetChunkAtPos((int)cursorPos.x, (int)cursorPos.y, out _, out _).IsInitialized)
        {
            var soil = World.GetSoilTileAt((int)cursorPos.x, (int)cursorPos.y);
            DrawTileImage(soil);
            SpriteBatches.UI.DrawString(Textures.Font, soil.GetType().Name, new Vector2(32, 25), Color.AliceBlue);
            var feature = World.GetFeatureTileAt((int)cursorPos.x, (int)cursorPos.y);
            if (feature is not null)
            {
                DrawTileImage(feature);
                SpriteBatches.UI.DrawString(Textures.Font, feature.GetType().Name, new Vector2(32, 50), Color.AliceBlue);
                if (feature is Tiles.IOre ore)
                {
                    var size = Textures.Font.MeasureString(ore.GetType().Name);
                    SpriteBatches.UI.DrawString(Textures.Font, $" ({ore.Quantity})", new Vector2(32 + size.X, 50), Color.White);
                }
            }
            SpriteBatches.UI.DrawString(Textures.Font, World.GetBiomeAt((int)cursorPos.x, (int)cursorPos.y).GetType().Name, new Vector2(32, 0), Color.AliceBlue);

            static void DrawTileImage(Tiles.Tile tile)
            {
                if (tile.Texture is not null)
                    SpriteBatches.UI.Draw(tile.Texture, new Rectangle(new(0), new(32)), tile.TextureRect, Color.White);
                else
                    SpriteBatches.UI.Draw(SpriteBatches.Pixel, new Rectangle(new(0), new(32)), tile.Color);
            }
        }

        for (int x = TopLeftWorldPos.X - 1; x <= BottomRightWorldPos.X; x++)
            for (int y = TopLeftWorldPos.Y - 1; y <= BottomRightWorldPos.Y; y++)
                DrawTile(x, y);

        base.Draw(gameTime);

        void DrawTile(int x, int y)
        {
            var soil = World.GetSoilTileAt(x, y);
            var feature = World.GetFeatureTileAt(x, y);
            var (x1, y1) = WorldToScreen(x, y);

            if (soil.Texture is { } textureSoil)
                SpriteBatches.Game.Draw(textureSoil, new Rectangle(x1, y1, TileSize, TileSize), soil.TextureRect, Color.White);
            else
                SpriteBatches.Game.Draw(SpriteBatches.Pixel, new Rectangle(x1, y1, TileSize, TileSize), soil.Color);

            if (feature?.Texture is { } textureFeature)
                SpriteBatches.Game.Draw(textureFeature, new Rectangle(x1, y1, TileSize, TileSize), feature.TextureRect, Color.White);
            else if (feature is not null)
                SpriteBatches.Game.Draw(SpriteBatches.Pixel, new Rectangle(x1, y1, TileSize, TileSize), feature.Color);

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
