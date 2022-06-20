using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.Components;

public class UpdatePerSec : IGameComponent, IUpdateable, IDrawable
{
    public bool Enabled => true;

    public int UpdateOrder => 0;

    public int DrawOrder => 0;

    public bool Visible => true;

    public event EventHandler<EventArgs>? EnabledChanged;
    public event EventHandler<EventArgs>? UpdateOrderChanged;
    public event EventHandler<EventArgs>? DrawOrderChanged;
    public event EventHandler<EventArgs>? VisibleChanged;
    private TimeSpan _ups, _fps;
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteFont _font;

    public UpdatePerSec(SpriteBatch spriteBatch, ContentManager content)
    {
        this._spriteBatch = spriteBatch;
        _font = content.Load<SpriteFont>(@"Fonts/Font");
    }

    public void Initialize()
    { }

    public void Update(GameTime gameTime)
    {
        _ups = gameTime.ElapsedGameTime;
    }

    public void Draw(GameTime gameTime)
    {
        _fps = gameTime.ElapsedGameTime;
        var windowSize = _spriteBatch.GraphicsDevice.Viewport.TitleSafeArea.Size;
        _spriteBatch.DrawCenteredString(_font, $"{1 / _fps.TotalSeconds:00}FPS", new(windowSize.X, 0), new(1, 0), Color.White);
        _spriteBatch.DrawCenteredString(_font, $"{1 / _ups.TotalSeconds:00}UPS", new(windowSize.X, 0), new(1, -1), Color.White);
    }
}