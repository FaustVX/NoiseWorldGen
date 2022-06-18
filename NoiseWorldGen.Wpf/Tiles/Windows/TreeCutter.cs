using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.Tiles.Windows;

public class TreeCutter : TickedFeatureTileWindow
{
    private sealed class ItemStackVM : MonoGameControls.MonoGameViewModel
    {
        public ItemStackVM(ItemStack stack, UIElement space, Window window)
        {
            Stack = stack;
            _space = space;
            _window = window;
        }

        public ItemStack Stack { get; }
        private SpriteBatch _sb = default!;
        private Texture2D _pixel = default!;
        private readonly UIElement _space;
        private readonly Window _window;

        public override void Initialize()
        {
            base.Initialize();
            _sb = new(GraphicsDevice);
            _pixel = new(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });
        }

        public override void Draw(GameTime gameTime)
        {
            var _drawableArea = new System.Windows.Rect(_space.TranslatePoint(new(0, 0), _window), _space.RenderSize);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _sb.Begin(transformMatrix: Matrix.CreateTranslation((float)_drawableArea.Left, (float)_drawableArea.Top, 0));
            _sb.Draw(_pixel, new Rectangle(new(5), new(500)), Color.Black);
            _sb.End();
        }
    }
    public new Tiles.TreeCutter Tile => (Tiles.TreeCutter)base.Tile;
    protected readonly MonoGameControls.MonoGameContentControl _item;
    private readonly ItemStackVM _itemVM;
    private readonly Panel _space;
    public TreeCutter(Tiles.TreeCutter tile)
        : base(tile)
    {
        _item = new()
        {
            Window = this,
        };
        Content = null;
        Content = new Grid()
        {
            Children =
            {
                _item,
                new Grid()
                {
                    RowDefinitions =
                    {
                        new()
                        {
                            Height = System.Windows.GridLength.Auto,
                        },
                        new()
                        {
                            Height = new(1, System.Windows.GridUnitType.Star),
                        },
                    },
                    Children =
                    {
                        _panel,
                        (_space = new Grid()),
                    },
                },
            },
        };
        _space.SetValue(Grid.RowProperty, 1);
        _item.DataContext = _itemVM = new(Tile.SapplingStored, _space, this);
    }
}