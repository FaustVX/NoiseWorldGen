using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoiseWorldGen.OpenGL.Tiles;

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
    private readonly Texture2D _pixel;
    private readonly SpriteBatch _spriteBatch;
    private KeyboardState _lastKeyboard = default!, _currentKeyboard = default!;

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
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        TileSize = 16;
        SetViewSize();
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

    protected override void Update(GameTime gameTime)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();

        if (IsDown(Keys.Escape))
            Exit();

        var speed = _world.Player.Speed;
        if (IsDown(Keys.RightShift) || IsDown(Keys.LeftShift))
            speed *= _world.Player.SpeedMultipler;
        if (_world.Player.IsFlying)
            speed *= _world.Player.FlySpeedMultiplier;
        var pos = _world.Player.Position with
        {
            X = _world.Player.Position.X + speed * XorFunc(IsDown, Keys.Q, Keys.Left, Keys.D, Keys.Right),
            Y = _world.Player.Position.Y + speed * XorFunc(IsDown, Keys.Z, Keys.Up, Keys.S, Keys.Down),
        };
        if (_world.Player.IsFlying || _world.GetTileAt((int)pos.X, (int)pos.Y) is Tile.IsWalkable)
            _world.Player.Position = pos;

        TileSize += XorFunc(IsClicked, Keys.Subtract, Keys.Add);

        ShowChunkBorders ^= (OrFunc(IsDown, Keys.LeftAlt, Keys.RightAlt) && IsClicked(Keys.F1));
        _world.Player.IsFlying ^= IsClicked(Keys.Space);

        SetBounds();

        base.Update(gameTime);
    }

    public bool IsClicked(Keys key)
        => _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    public bool IsDown(Keys key)
        => _currentKeyboard.IsKeyDown(key);

    public bool OrFunc<T>(Func<T, bool> func, T key1, T key2)
        => func(key1) || func(key2);

    public int XorFunc<T>(Func<T, bool> keyFunc, T key1, T key2)
        => (keyFunc(key1), keyFunc(key2)) switch
        {
            (true, false) => -1,
            (false, true) => +1,
            _ => 0,
        };

    public int XorFunc<T>(Func<T, bool> keyFunc, T key1, T key1Alt, T key2, T key2Alt)
        => (keyFunc(key1) || keyFunc(key1Alt), keyFunc(key2) || keyFunc(key2Alt)) switch
        {
            (true, false) => -1,
            (false, true) => +1,
            _ => 0,
        };

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        for (int x = TopLeftWorldPos.X - 1; x <= BottomRightWorldPos.X; x++)
            for (int y = TopLeftWorldPos.Y - 1; y <= BottomRightWorldPos.Y; y++)
                DrawTile(x, y);

        _spriteBatch.End();

        base.Draw(gameTime);

        void DrawTile(int x, int y)
        {
            var color = _world.GetTileAt(x, y) switch
            {
                Mountain => Color.DimGray,
                FrozenMountain => Color.WhiteSmoke,
                Stone => Color.DarkGray,
                FrozenWater => Color.CornflowerBlue,
                Water or RiverWater => Color.DarkBlue,
                IronOre => Color.Red,
                CoalOre => Color.Black,
                Tree => Color.Green,
                _ => Color.Transparent,
            };
            var x1 = (int)((x - _world.Player.Position.X + ViewSize.Width / 2f) * TileSize);
            var y1 = (int)((y - _world.Player.Position.Y + ViewSize.Height / 2f) * TileSize);
            _spriteBatch.Draw(_pixel, new Rectangle(x1, y1, TileSize, TileSize), color);
            if (ShowChunkBorders)
            {
                if (x % Chunck.Size == 0)
                    _spriteBatch.Draw(_pixel, new Rectangle(x1, y1, 1, TileSize), Color.Black);
                if (y % Chunck.Size == 0)
                    _spriteBatch.Draw(_pixel, new Rectangle(x1, y1, TileSize, 1), Color.Black);
            }
        }
    }
}
