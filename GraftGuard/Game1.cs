using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard
{
    enum GameState {
        Night,
        Dawn,
        Day
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameState state;
        private double timer;

        private static readonly double NightTimeLength = 5;
        private static readonly double DawnTimeLength = 5;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.state = GameState.Night;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (state) {
                case GameState.Night:
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer <= 0) {
                        state = GameState.Dawn;
                        timer = DawnTimeLength;
                    }
                    break;

                case GameState.Dawn:
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer <= 0) {
                        state = GameState.Day;
                    }
                    break;

                case GameState.Day:
                    break;
            }

            // TODO: call Update for all GameObjects here

            base.Update(gameTime);
        }

        /// <summary>
        /// Start Night
        /// </summary>
        public void StartNight() {
            state = GameState.Night;
            timer = NightTimeLength;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // TODO: call Draw for all GameObjects here

            base.Draw(gameTime);
        }
    }
}
