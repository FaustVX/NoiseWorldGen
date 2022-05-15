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

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    public World World { get; }

    /// <summary>
    /// Top Left Tile
    /// </summary>
    public Point TopLeftWorldPos { get; set; }

    /// <summary>
    /// Bottom Right Tile
    /// </summary>
    public Point BottomRightWorldPos { get; set; }

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
    public Point ViewSize { get; set; }
    public Point WindowSize
        => GraphicsDevice.Viewport.TitleSafeArea.Size;
    public bool ShowChunkBorders { get; set; }
    private bool showUI = true;
    private readonly SpriteBatch _tempUI;
    public bool ShowUI
    {
        get => showUI;
        set
        {
            if (value == showUI)
                return;
            showUI = value;
            if (showUI)
                SpriteBatches.UI = _tempUI;
            else
                SpriteBatches.UI = null;
        }
    }
    private bool _isFullScreen = false;
    private Point _defaultWindowSize;
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
        _graphics = new GraphicsDeviceManager(this);
        _graphics.ApplyChanges();
        _defaultWindowSize = new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (s, e) => SetViewSize();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        SpriteBatches.Game = new(GraphicsDevice);
        SpriteBatches.UI = _tempUI = new(GraphicsDevice);
        SpriteBatches.Pixel = new Texture2D(GraphicsDevice, 1, 1);
        SpriteBatches.Pixel.SetData(new[] { Color.White });
        World = new World(new Random().Next());

        TileSize = 32;
        SetViewSize();

        Components.Add(World);
        Components.Add(World.Player);
        Components.Add(Keyboard.Instance);
    }

    [MemberNotNull(nameof(TopLeftWorldPos), nameof(BottomRightWorldPos))]
    [MemberNotNull(nameof(ViewSize))]
    private void SetViewSize()
    {
        ViewSize = WindowSize / new Point(TileSize);
        SetBounds();
    }

    [MemberNotNull(nameof(TopLeftWorldPos), nameof(BottomRightWorldPos))]
    private void SetBounds()
    {
        var (tl, br) = (TopLeftWorldPos, BottomRightWorldPos);
        TopLeftWorldPos = new((int)(World.Player.Position.X - ViewSize.X / 2), (int)(World.Player.Position.Y - ViewSize.Y / 2));
        BottomRightWorldPos = new(TopLeftWorldPos.X + ViewSize.X, TopLeftWorldPos.Y + ViewSize.Y);
        if (tl == default && br == default)
            return;
        for (var x = tl.X; x < br.X; x++)
            for (var y = tl.Y; y < br.Y; y++)
                World.GetChunkAtPos(x, y, out _, out _).IsActive = false;
        for (var x = TopLeftWorldPos.X; x < BottomRightWorldPos.X; x++)
            for (var y = TopLeftWorldPos.Y; y < BottomRightWorldPos.Y; y++)
                World.GetChunkAtPos(x, y, out _, out _).IsActive = true;
    }

    public (int x, int y) WorldToScreen(float x, float y)
    {
        var x1 = (int)((x - World.Player.Position.X + ViewSize.X / 2f) * TileSize);
        var y1 = (int)((y - World.Player.Position.Y + ViewSize.Y / 2f) * TileSize);
        return (x1, y1);
    }

    public (float x, float y) ScreenToWorld(int x, int y)
    {
        var x1 = ((float)x / TileSize) - ViewSize.X / 2f + World.Player.Position.X;
        var y1 = ((float)y / TileSize) - ViewSize.Y / 2f + World.Player.Position.Y;
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
        var mouseState = Mouse.GetState();
        var cursorPos = ScreenToWorld(mouseState.Position.X, mouseState.Position.Y);
        if (Keyboard.Instance.IsDown(Keys.Escape))
            Exit();

        TileSize += Keyboard.XorFunc(Keyboard.Instance.IsClicked, Keys.Subtract, Keys.Add);

        ShowUI ^= (Keyboard.Instance.IsClicked(Keys.F1, isExclusive: true));
        ShowChunkBorders ^= (Keyboard.Instance.IsClicked(Keys.F1) && Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftAlt, Keys.RightAlt));
        IsFullScreen ^= (Keyboard.Instance.IsClicked(Keys.Enter) && Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftControl, Keys.RightControl));
        if (Keyboard.Instance.IsClicked(Keys.Tab, isExclusive: true))
            TileTemplates.CurrentIndex = (TileTemplates.CurrentIndex + 1) % TileTemplates.Tiles.Count;
        if (mouseState.LeftButton is Microsoft.Xna.Framework.Input.ButtonState.Pressed && World.GetChunkAtPos((int)cursorPos.x, (int)cursorPos.y, out _ ,out _).IsActive)
        {
            switch (TileTemplates.CurrentTemplate.Create())
            {
                case Tiles.SoilTile tile when tile is Tiles.Tile.IsFeaturePlacable || World.GetFeatureTileAt((int)cursorPos.x, (int)cursorPos.y) is null:
                    World.SetSoilTileAt((int)cursorPos.x, (int)cursorPos.y, tile);
                    break;
                case Tiles.FeatureTile tile when World.GetSoilTileAt((int)cursorPos.x, (int)cursorPos.y) is Tiles.Tile.IsFeaturePlacable:
                    World.SetFeatureTileAt((int)cursorPos.x, (int)cursorPos.y, tile);
                    break;
            }
        }


        for (int x = TopLeftWorldPos.X - 9; x <= BottomRightWorldPos.X + 9; x += 8)
            for (int y = TopLeftWorldPos.Y - 9; y <= BottomRightWorldPos.Y + 9; y += 8)
                World.GetChunkAtPos(x, y, out _, out _).Initialize();

        base.Update(gameTime);

        SetBounds();
    }

    protected override bool BeginDraw()
    {
        SpriteBatches.Game.Begin();
        SpriteBatches.UI?.Begin();
        return base.BeginDraw();
    }

    protected override void EndDraw()
    {
        SpriteBatches.Game.End();
        SpriteBatches.UI?.End();
        base.EndDraw();
    }

    protected override void Draw(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        var cursorPos = ScreenToWorld(mouseState.Position.X, mouseState.Position.Y);
        if (SpriteBatches.UI is not null && World.GetChunkAtPos((int)cursorPos.x, (int)cursorPos.y, out _, out _).IsInitialized)
        {
            var soil = World.GetSoilTileAt((int)cursorPos.x, (int)cursorPos.y);
            DrawTileImage(soil);
            SpriteBatches.UI.DrawString(Textures.Font, soil.Name, new Vector2(32, 25), Color.AliceBlue);
            var feature = World.GetFeatureTileAt((int)cursorPos.x, (int)cursorPos.y);
            if (feature is not null)
            {
                DrawTileImage(feature);
                SpriteBatches.UI.DrawString(Textures.Font, feature.Name, new Vector2(32, 50), Color.AliceBlue);
            }
            SpriteBatches.UI.DrawString(Textures.Font, World.GetBiomeAt((int)cursorPos.x, (int)cursorPos.y).Name, new Vector2(32, 0), Color.AliceBlue);

            static void DrawTileImage(Tiles.Tile tile)
            {
                if (tile.Texture is not null)
                    SpriteBatches.UI!.Draw(tile.Texture, new Rectangle(new(0), new(32)), tile.TextureRect, Color.White);
                else
                    SpriteBatches.UI!.Draw(SpriteBatches.Pixel, new Rectangle(new(0), new(32)), tile.Color);
            }
            var currentTilePos = WorldToScreen((int)cursorPos.x, (int)cursorPos.y);
            SpriteBatches.UI.Draw(SpriteBatches.Pixel, new Rectangle(new(currentTilePos.x, currentTilePos.y), new(tileSize)), Color.Wheat * .25f);
            DrawHotBar();

            void DrawHotBar()
            {
                var length = TileTemplates.Tiles.Count;
                var tl = new Point(WindowSize.X / 2 - length * TileSize / 2, WindowSize.Y - TileSize);
                SpriteBatches.UI!.Draw(SpriteBatches.Pixel, new Rectangle(tl - new Point(5), new Point(TileSize * length + 10, TileSize + 5)), Color.Gray * .75f);
                foreach (var template in TileTemplates.Tiles)
                {
                    var color = template == TileTemplates.CurrentTemplate ? Color.White * .5f : Color.White;
                    SpriteBatches.UI!.Draw(template.Texture, new Rectangle(tl, new(TileSize)), color);
                    tl += new Point(TileSize, 0);
                }
            }
        }

        for (int x = TopLeftWorldPos.X - 1; x <= BottomRightWorldPos.X + 1; x++)
            for (int y = TopLeftWorldPos.Y - 1; y <= BottomRightWorldPos.Y + 1; y++)
                DrawTile(x, y);

        base.Draw(gameTime);

        void DrawTile(int x, int y)
        {
            var soil = World.GetSoilTileAt(x, y);
            var feature = World.GetFeatureTileAt(x, y);
            var (x1, y1) = WorldToScreen(x, y);

            DrawTileImage(soil);
            DrawTileImage(feature);

            void DrawTileImage(Tiles.Tile? tile)
            {
                if (tile?.Texture is not null)
                    SpriteBatches.Game.Draw(tile.Texture, new Rectangle(x1, y1, TileSize, TileSize), tile.TextureRect, Color.White);
                else if (tile is not null)
                    SpriteBatches.Game.Draw(SpriteBatches.Pixel, new Rectangle(x1, y1, TileSize, TileSize), tile.Color);
            }

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
