using NoiseWorldGen.Wpf.MonoGameControls;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoiseWorldGen.Wpf.Content;
using static NoiseWorldGen.Wpf.Inputs.Keyboard;
using static NoiseWorldGen.Wpf.Inputs.Mouse;
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

    private Tiles.Windows.FeatureTileWindow? _windowToShow;

    public MainWindowViewModel()
    {
        World = new World(new Random().Next());
    }

    private bool _showUI = true;
    [MaybeNull]
    private SpriteBatch UI
        => _showUI ? base[nameof(UI)] : null;
    private SpriteBatch Game
        => base[nameof(Game)];

    public override void Initialize()
    {
        base.Initialize();
        Content.RootDirectory = "Content";
        OnCreateGraphicDevice?.Invoke(GraphicsDevice);

        CreateSpriteBatch(nameof(Game));
        CreateSpriteBatch(nameof(UI));

        TileSize = 32;

        Components.Add(World);
        Components.Add(World.Player);
        Components.Add(new Components.UpdatePerSec(UI!, Content));
        Inputs.Keyboard.RegisterKey(Key.Tab);
        Inputs.Keyboard.RegisterKey(Key.F1);
        Inputs.Keyboard.RegisterKey(Key.LeftAlt);
        Inputs.Keyboard.RegisterKey(Key.RightAlt);
        Inputs.Keyboard.RegisterKey(Key.LeftShift);
        Inputs.Keyboard.RegisterKey(Key.RightShift);
        Inputs.Keyboard.RegisterKey(Key.E);

        PostInitialize();
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
    }

    public override void AfterRender()
    {
        _windowToShow?.Show();
        _windowToShow = null;
    }

    public override void Update(GameTime gameTime)
    {
        SetViewSize();
        var cursorPos = ScreenToWorld(MousePosition).ToPoint();

        if (Key.E.IsPressed())
        {
            if (World.GetFeatureTileAt(cursorPos) is Tiles.Windows.IWindow { WindowType: var t } and Tiles.FeatureTile ft)
            {
                _windowToShow = (Tiles.Windows.FeatureTileWindow)Activator.CreateInstance(t, new object[] { ft })!;
            }
        }

        if (Key.F1.IsPressed())
            if ((Key.LeftAlt, Key.RightAlt).IsDown())
                ShowChunkBorders ^= true;
            else
                _showUI ^= true;

        if (Key.Tab.IsPressed())
        {
            var offset = (Key.LeftShift, Key.RightShift).IsDown() ? -1 : +1;
            TileTemplates.CurrentIndex = ProperRemainder(TileTemplates.CurrentIndex + offset, TileTemplates.Tiles.Count).remainder;
        }
        else if (ScroolWheelDirection < 0)
            TileTemplates.CurrentIndex = ProperRemainder(TileTemplates.CurrentIndex - 1, TileTemplates.Tiles.Count).remainder;
        else if (ScroolWheelDirection > 0)
            TileTemplates.CurrentIndex = ProperRemainder(TileTemplates.CurrentIndex + 1, TileTemplates.Tiles.Count).remainder;

        if (World.GetChunkAtPos(cursorPos, out _, out _).IsActive)
        {
            if (Button.LeftClick.IsPressed())
            {
                var tile = TileTemplates.CurrentTemplate switch
                {
                    TileTemplate.Static s => s.Create(),
                    TileTemplate.Dynamic d => d.Create(World, cursorPos),
                };
                World.PlaceTile(cursorPos, tile);
            }
            if (Button.RightClick.IsPressed())
                World.SetFeatureTileAt(cursorPos, null);
        }

        for (int x = TopLeftWorldPos.X - 9; x <= BottomRightWorldPos.X + 9; x += 8)
            for (int y = TopLeftWorldPos.Y - 9; y <= BottomRightWorldPos.Y + 9; y += 8)
                World.GetChunkAtPos(new(x, y), out _, out _).Initialize();

        base.Update(gameTime);
        Inputs.Keyboard.Update();
        Inputs.Mouse.Update();

        SetBounds();
    }

    public override void Draw(GameTime gameTime)
    {
        DrawUI();

        for (int x = TopLeftWorldPos.X - 1; x <= BottomRightWorldPos.X + 1; x++)
            for (int y = TopLeftWorldPos.Y - 1; y <= BottomRightWorldPos.Y + 1; y++)
                DrawTile(new(x, y));
    }

    private void DrawTile(Point pos)
    {
        var soil = World.GetSoilTileAt(pos);
        var feature = World.GetFeatureTileAt(pos);
        var screen = WorldToScreen(pos.ToVector2());

        soil.Draw(new(screen, new(TileSize)), World, pos, Game);
        feature?.Draw(new(screen, new(TileSize)), World, pos, Game);

        if (ShowChunkBorders && UI is {} sb)
        {
            if (pos.X % Chunck.Size == 0)
                sb.Draw(sb.Pixel(), new Rectangle(screen, new(1, TileSize)), Color.Black);
            if (pos.Y % Chunck.Size == 0)
                sb.Draw(sb.Pixel(), new Rectangle(screen, new(TileSize, 1)), Color.Black);
        }
    }

    private void DrawUI()
    {
        if (UI is not { } sb)
            return;

        DrawTileInfo(sb);

        DrawHotBar(sb);

        DrawNetworks(sb);

        void DrawTileInfo(SpriteBatch sb)
        {
            var cursorPos = ScreenToWorld(MousePosition).ToPoint();
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

                void DrawTileImage(Tiles.Tile tile, SpriteBatch sb)
                {
                    if (tile.Texture is not null)
                        sb.Draw(tile.Texture, new Rectangle(new(0), new(32)), tile.TextureRect, Color.White);
                    else
                        sb.Draw(sb.Pixel(), new Rectangle(new(0), new(32)), tile.Color);
                }
                var currentTilePos = WorldToScreen(cursorPos.ToVector2());
                sb.Draw(sb.Pixel(), new Rectangle(currentTilePos, new(tileSize)), Color.Wheat * .25f);
            }
        }

        void DrawHotBar(SpriteBatch sb)
        {
            var length = TileTemplates.Tiles.Count;
            var tl = new Point(WindowSize.X / 2 - length * TileSize / 2, WindowSize.Y - TileSize);
            sb.Draw(sb.Pixel(), new Rectangle(tl - new Point(5, TileSize + 5), new Point(TileSize * length + 10, TileSize * 2 + 5)), Color.Gray * .75f);
            sb.DrawCenteredString(Textures.Font, TileTemplates.CurrentTemplate.Name, new Rectangle(tl - new Point(5, TileSize), new Point(TileSize * length + 10, TileSize * 2 + 5)).Center, new(.5f, 1), Color.White);
            foreach (var template in TileTemplates.Tiles)
            {
                var color = template == TileTemplates.CurrentTemplate ? Color.White * .5f : Color.White;
                sb.Draw(template.Texture, new Rectangle(tl, new(TileSize)), color);
                tl += new Point(TileSize, 0);
            }
        }

        void DrawNetworks(SpriteBatch sb)
        {
            foreach (var network in Networks.Network.Networks)
                foreach (var tile in network.Tiles)
                    foreach (var connection in network.GetConnection(tile))
                        sb.DrawLine(WorldToScreen(tile.Pos.ToVector2() + Vector2.One / 2), WorldToScreen(connection.Pos.ToVector2() + Vector2.One / 2), Color.SkyBlue);
        }
    }
}
