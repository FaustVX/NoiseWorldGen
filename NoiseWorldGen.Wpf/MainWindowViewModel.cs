using NoiseWorldGen.Wpf.MonoGameControls;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoiseWorldGen.Wpf.Content;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;
using Key = System.Windows.Input.Key;

namespace NoiseWorldGen.Wpf;

public class MainWindowViewModel : MonoGameViewModel
{
    public static event Action<GraphicsDevice>? OnCreateGraphicDevice;
    public static event Action<ContentManager>? OnLoadContent;
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

    private Microsoft.Xna.Framework.Input.MouseState _currentMouse = Mouse.GetState();

    private TimeSpan _ups, _fps;

    public MainWindowViewModel()
    {
        _tempUI = SpriteBatches.UI!;
        World = new World(new Random().Next());
    }

    public override void Initialize()
    {
        base.Initialize();
        Content.RootDirectory = "Content";
        OnCreateGraphicDevice?.Invoke(GraphicsDevice);

        TileSize = 32;
        SetViewSize();

        Components.Add(World);
        Components.Add(World.Player);
    }

    public override void SizeChanged(object sender, System.Windows.SizeChangedEventArgs args)
        => SetViewSize();

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

    public override void LoadContent()
    {
        OnLoadContent?.Invoke(Content);
        Microsoft.Xna.Framework.Audio.SoundEffect.DistanceScale = 000000010000000f;
    }

    public override void Update(GameTime gameTime)
    {
        (var oldMouseState, _currentMouse) = (_currentMouse, Mouse.GetState());
        _ups = gameTime.ElapsedGameTime;
        var cursorPos = ScreenToWorld(oldMouseState.Position).ToPoint();

        TileSize += (Key.Add, Key.Subtract).IsKeyXor(Extensions.IsKeyClicked);

        if (Key.F1.IsKeyClicked())
            if ((Key.LeftAlt, Key.RightAlt).IsKeyDown())
                ShowChunkBorders ^= true;
            else
                ShowUI ^= true;

        if (Key.Tab.IsKeyClicked())
        {
            var offset = (Key.LeftShift, Key.RightShift).IsKeyDown() ? -1 : +1;
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
                World.PlaceTile(cursorPos, tile);
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

    public override bool BeginDraw()
    {
        SpriteBatches.Game.Begin();
        SpriteBatches.UI?.Begin();
        return true;
    }

    public override void EndDraw()
    {
        SpriteBatches.Game.End();
        SpriteBatches.UI?.End();
    }

    public override void Draw(GameTime gameTime)
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

            foreach (var network in Networks.Network.Networks)
                foreach (var tile in network.Tiles)
                    foreach (var connection in network.GetConnection(tile))
                            sb.DrawLine(WorldToScreen(tile.Pos.ToVector2() + Vector2.One / 2), WorldToScreen(connection.Pos.ToVector2() + Vector2.One / 2), Color.SkyBlue);
        }

        for (int x = TopLeftWorldPos.X - 1; x <= BottomRightWorldPos.X + 1; x++)
            for (int y = TopLeftWorldPos.Y - 1; y <= BottomRightWorldPos.Y + 1; y++)
                DrawTile(new(x, y));

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
