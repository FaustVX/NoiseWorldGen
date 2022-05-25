using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoiseWorldGen.OpenGL.Content;
using NoiseWorldGen.OpenGL.Inputs;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace NoiseWorldGen.OpenGL;

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

    public bool IsFocused { get; private set; }

    private Microsoft.Xna.Framework.Input.MouseState _currentMouse = Mouse.GetState();

    private TimeSpan _ups, _fps;

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

        Activated += (_, _) => IsFocused = true;
        Deactivated += (_, _) => IsFocused = false;
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
                World.GetChunkAtPos(new(x, y), out _, out _).IsActive = false;
        for (var x = TopLeftWorldPos.X; x < BottomRightWorldPos.X; x++)
            for (var y = TopLeftWorldPos.Y; y < BottomRightWorldPos.Y; y++)
                World.GetChunkAtPos(new(x, y), out _, out _).IsActive = true;
    }

    public Point WorldToScreen(Vector2 pos)
    {
        var x1 = (int)((pos.X - World.Player.Position.X + ViewSize.X / 2f) * TileSize);
        var y1 = (int)((pos.Y - World.Player.Position.Y + ViewSize.Y / 2f) * TileSize);
        return new(x1, y1);
    }

    public Vector2 ScreenToWorld(Point pos)
    {
        var x1 = ((float)pos.X / TileSize) - ViewSize.X / 2f + World.Player.Position.X;
        var y1 = ((float)pos.Y / TileSize) - ViewSize.Y / 2f + World.Player.Position.Y;
        if (x1 < 0)
            x1--;
        if (y1 < 0)
            y1--;
        return new(x1, y1);
    }

    protected override void LoadContent()
    {
        Textures.Ores = Content.Load<Texture2D>($@"{nameof(OpenGL.Content.SpriteSheets)}\{nameof(OpenGL.Content.SpriteSheets.Ores)}");
        Textures.Font = Content.Load<SpriteFont>($@"Fonts\{nameof(OpenGL.Content.Textures.Font)}");
    }

    protected override void Update(GameTime gameTime)
    {
        (var oldMouseState, _currentMouse) = (_currentMouse, Mouse.GetState());
        if (!IsFocused)
            return;
        _ups = gameTime.ElapsedGameTime;
        var cursorPos = ScreenToWorld(oldMouseState.Position).ToPoint();
        if (Keyboard.Instance.IsDown(Keys.Escape))
            Exit();

        TileSize += Keyboard.XorFunc(Keyboard.Instance.IsClicked, Keys.Subtract, Keys.Add);

        ShowUI ^= (Keyboard.Instance.IsClicked(Keys.F1, isExclusive: true));
        ShowChunkBorders ^= (Keyboard.Instance.IsClicked(Keys.F1) && Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftAlt, Keys.RightAlt));
        IsFullScreen ^= (Keyboard.Instance.IsClicked(Keys.Enter) && Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftControl, Keys.RightControl));
        if (Keyboard.Instance.IsClicked(Keys.Tab))
        {
            var offset = Keyboard.OrFunc(Keyboard.Instance.IsDown, Keys.LeftShift, Keys.RightShift) ? -1 : +1;
            TileTemplates.CurrentIndex = ProperRemainder(TileTemplates.CurrentIndex + offset, TileTemplates.Tiles.Count).remainder;
        }
        else if (oldMouseState.ScrollWheelValue < _currentMouse.ScrollWheelValue)
            TileTemplates.CurrentIndex = ProperRemainder(TileTemplates.CurrentIndex - 1, TileTemplates.Tiles.Count).remainder;
        else if (oldMouseState.ScrollWheelValue > _currentMouse.ScrollWheelValue)
            TileTemplates.CurrentIndex = ProperRemainder(TileTemplates.CurrentIndex + 1, TileTemplates.Tiles.Count).remainder;

        if (World.GetChunkAtPos(cursorPos, out _, out _).IsActive)
        {
            if (oldMouseState.LeftButton is Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                var tile = TileTemplates.CurrentTemplate switch
                {
                    TileTemplate.Static s => s.Create(),
                    TileTemplate.Dynamic d => d.Create(World, cursorPos),
                };
                switch (tile)
                {
                    case Tiles.SoilTile st when st is Tiles.Tile.IsFeaturePlacable || World.GetFeatureTileAt(cursorPos) is null:
                        World.SetSoilTileAt(cursorPos, st);
                        break;
                    case Tiles.FeatureTile ft when World.GetSoilTileAt(cursorPos) is Tiles.Tile.IsFeaturePlacable:
                        World.SetFeatureTileAt(cursorPos, ft);
                        break;
                }
            }
            if (oldMouseState.RightButton is Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                World.SetFeatureTileAt(cursorPos, null);
        }

        for (int x = TopLeftWorldPos.X - 9; x <= BottomRightWorldPos.X + 9; x += 8)
            for (int y = TopLeftWorldPos.Y - 9; y <= BottomRightWorldPos.Y + 9; y += 8)
                World.GetChunkAtPos(new(x, y), out _, out _).Initialize();

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
        _fps = gameTime.ElapsedGameTime;
        var cursorPos = ScreenToWorld(_currentMouse.Position).ToPoint();
        if (SpriteBatches.UI is {} sb)
        {
            if (World.GetChunkAtPos(cursorPos, out _, out _).IsActive)
            {
                var soil = World.GetSoilTileAt(cursorPos);
                DrawTileImage(soil, sb);
                sb.DrawString(Textures.Font, soil.Name, new Vector2(32, 25), Color.AliceBlue);
                var feature = World.GetFeatureTileAt(cursorPos);
                if (feature is not null)
                {
                    DrawTileImage(feature, sb);
                    sb.DrawString(Textures.Font, feature.Name, new Vector2(32, 50), Color.AliceBlue);
                }
                sb.DrawString(Textures.Font, World.GetBiomeAt(cursorPos).Name, new Vector2(32, 0), Color.AliceBlue);

                static void DrawTileImage(Tiles.Tile tile, SpriteBatch sb)
                {
                    if (tile.Texture is not null)
                        sb.Draw(tile.Texture, new Rectangle(new(0), new(32)), tile.TextureRect, Color.White);
                    else
                        sb.Draw(SpriteBatches.Pixel, new Rectangle(new(0), new(32)), tile.Color);
                }
                var currentTilePos = WorldToScreen(cursorPos.ToVector2());
                sb.Draw(SpriteBatches.Pixel, new Rectangle(currentTilePos, new(tileSize)), Color.Wheat * .25f);
            }

            var length = TileTemplates.Tiles.Count;
            var tl = new Point(WindowSize.X / 2 - length * TileSize / 2, WindowSize.Y - TileSize);
            sb.Draw(SpriteBatches.Pixel, new Rectangle(tl - new Point(5, TileSize + 5), new Point(TileSize * length + 10, TileSize * 2 + 5)), Color.Gray * .75f);
            sb.DrawCenteredString(Textures.Font, TileTemplates.CurrentTemplate.Name, new Rectangle(tl - new Point(5, TileSize), new Point(TileSize * length + 10, TileSize * 2 + 5)).Center, new(.5f, 1), Color.White);
            foreach (var template in TileTemplates.Tiles)
            {
                var color = template == TileTemplates.CurrentTemplate ? Color.White * .5f : Color.White;
                sb.Draw(template.Texture, new Rectangle(tl, new(TileSize)), color);
                tl += new Point(TileSize, 0);
            }

            sb.DrawCenteredString(Textures.Font, $"{1/_fps.TotalSeconds:00}FPS", new(WindowSize.X, 0), new(1, 0), Color.White);
            sb.DrawCenteredString(Textures.Font, $"{1/_ups.TotalSeconds:00}UPS", new(WindowSize.X, 0), new(1, -1), Color.White);
        }

        for (int x = TopLeftWorldPos.X - 1; x <= BottomRightWorldPos.X + 1; x++)
            for (int y = TopLeftWorldPos.Y - 1; y <= BottomRightWorldPos.Y + 1; y++)
                DrawTile(new(x, y));

        base.Draw(gameTime);

        void DrawTile(Point pos)
        {
            var soil = World.GetSoilTileAt(pos);
            var feature = World.GetFeatureTileAt(pos);
            var screen = WorldToScreen(pos.ToVector2());

            soil.Draw(new(screen, new(TileSize)), World, pos);
            feature?.Draw(new(screen, new(TileSize)), World, pos);

            if (ShowChunkBorders && SpriteBatches.UI is {} sb)
            {
                if (pos.X % Chunck.Size == 0)
                    sb.Draw(SpriteBatches.Pixel, new Rectangle(screen, new(1, TileSize)), Color.Black);
                if (pos.Y % Chunck.Size == 0)
                    sb.Draw(SpriteBatches.Pixel, new Rectangle(screen, new(TileSize, 1)), Color.Black);
            }
        }
    }
}
