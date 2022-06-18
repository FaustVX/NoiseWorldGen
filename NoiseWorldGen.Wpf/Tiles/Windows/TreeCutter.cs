using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.Tiles.Windows;

public class TreeCutter : TickedFeatureTileWindow
{
    private sealed class ItemStackVM : MonoGameControls.MonoGameViewModelEmbeded
    {
        public ItemStackVM(ItemStack stack, UIElement space, FeatureTileWindow window)
            : base(window, space)
        {
            Stack = stack;
        }

        public ItemStack Stack { get; }
        private SpriteBatch _sb = default!;

        public override void Initialize()
        {
            base.Initialize();
            _sb = CreateSpriteBatch(nameof(_sb));
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _sb.Draw(_sb.Pixel(), new Rectangle(new(5), new(500)), Color.Black);
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