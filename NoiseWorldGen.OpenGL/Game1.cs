using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoiseWorldGen.Core;

namespace NoiseWorldGen.OpenGL
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly World _world;
        private readonly Texture2D _pixel;
        private readonly SpriteBatch _spriteBatch;
        private readonly int _blockSize;
        private readonly int _width, _height;
        private int _posX = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.ApplyChanges();
            _blockSize = 1;
            _height = Window.ClientBounds.Height / _blockSize;
            _width = Window.ClientBounds.Width / _blockSize;

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _world = new(Environment.TickCount, _height);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            // _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            // _graphics.IsFullScreen = true;
            // _graphics.ApplyChanges();
            // _blockSize = _graphics.PreferredBackBufferHeight / _height;

            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Left))
                _posX--;
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Right))
                _posX++;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var block = _world[x + _posX, y];
                    var color = block switch
                    {
                        Block.Air => Color.CornflowerBlue,
                        Block.Bedrock => Color.Black,
                        Block.Stone => Color.Gray,
                        Block.Water => Color.Blue,
                        Block.Dirt => Color.Green,
                        Block.Sand => Color.Gold,
                        Block.Ore => Color.IndianRed,
                        _ => throw new NotImplementedException(),
                    };
                    _spriteBatch.Draw(_pixel, new Rectangle(x * _blockSize, (Window.ClientBounds.Height - y - 1) * _blockSize, _blockSize, _blockSize), color);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
