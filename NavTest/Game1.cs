using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShapeUtils;
using System.Collections.Generic;

namespace NavTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Fields
        List<PathNode> route;
        Texture2D texture;
        Enemy enemy;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            route = new List<PathNode>();
            route.Add(new PathNode(new Vector2(100, 100)));
            route.Add(new PathNode(new Vector2(300, 10)));
            route.Add(new PathNode(new Vector2(500, 200)));
            enemy = new Enemy(new Vector2(0, 0), 3, texture, route);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            texture = Content.Load<Texture2D>("coin");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            enemy.Move();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            ShapeBatch.Begin(GraphicsDevice);
            foreach (PathNode node in route)
            {
                node.Draw();
            }
            ShapeBatch.End();

            _spriteBatch.Begin();

            _spriteBatch.Draw(texture, enemy.Position, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
