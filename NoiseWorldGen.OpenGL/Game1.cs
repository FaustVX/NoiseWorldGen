using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NoiseWorldGen.OpenGL;

public record class Point<T>(T X, T Y);
public record class Size<T>(T Width, T Height);

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly World _world;
    private readonly Texture2D _pixel;
    private readonly SpriteBatch _spriteBatch;
    private KeyboardState _lastKeyboard = default!, _currentKeyboard = default!;

    /// <summary>
    /// Pos in Tile Size
    /// 0.5 => Middle of Tile
    /// </summary>
    public Point<float> Pos { get; set; }

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

    public float Speed { get; } = 1;
    public float SpeedMultipler { get; } = 5;

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

        Pos = new(.5f, .5f);
        TileSize = 1;
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
        TopLeftWorldPos = new((int)(Pos.X - ViewSize.Width / 2) - 1, (int)(Pos.Y - ViewSize.Height / 2) - 1);
        BottomRightWorldPos = new(TopLeftWorldPos.X + ViewSize.Width + 2, TopLeftWorldPos.Y + ViewSize.Height + 2);
    }

    protected override void Update(GameTime gameTime)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();

        if (IsDown(Keys.Escape))
            Exit();

        var speed = IsDown(Keys.RightShift) || IsDown(Keys.LeftShift) ? Speed * SpeedMultipler : Speed;
        Pos = Pos with
        {
            X = Pos.X + speed * XorFunc(IsDown, Keys.Q, Keys.Left, Keys.D, Keys.Right),
            Y = Pos.Y + speed * XorFunc(IsDown, Keys.Z, Keys.Up, Keys.S, Keys.Down),
        };

        if (IsClicked(Keys.Add))
            TileSize++;
        if (IsClicked(Keys.Subtract))
            TileSize--;

        SetBounds();

        base.Update(gameTime);
    }

    public bool IsClicked(Keys key)
        => _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    public bool IsDown(Keys key)
        => _currentKeyboard.IsKeyDown(key);

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
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();

        for (int x = TopLeftWorldPos.X; x < BottomRightWorldPos.X; x++)
            for (int y = TopLeftWorldPos.Y; y < BottomRightWorldPos.Y; y++)
                DrawTile(x, y);

        _spriteBatch.End();

        base.Draw(gameTime);

        void DrawTile(int x, int y)
        {
            var color = _world[x, y] switch
            {
                Mountain => Color.DimGray,
                Stone => Color.DarkGray,
                ShallowWater => Color.Aqua,
                Water => Color.DarkBlue,
                DeepWater => Color.Blue,
                _ => Color.Transparent,
            };
            x = (int)((x - Pos.X + ViewSize.Width / 2f) * TileSize);
            y = (int)((y - Pos.Y + ViewSize.Height / 2f) * TileSize);
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, TileSize, TileSize), color);
            // _spriteBatch.Draw(_pixel, new Rectangle(x, y, TileSize.Width, 1), Color.Black);
            // _spriteBatch.Draw(_pixel, new Rectangle(x, y, 1, TileSize.Height), Color.Black);
        }
    }
}
