using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard
{
    enum TimeState {
        Night,
        Dawn,
        Day
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private TimeState timeState;
        private float timer;

        private static readonly float NightTimeLength = 5;
        private static readonly float DawnTimeLength = 5;
        private InputManager inputManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.timeState = TimeState.Night;
            inputManager = new InputManager();

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

            switch (timeState) {
                case TimeState.Night:
                    timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer <= 0) {
                        timeState = TimeState.Dawn;
                        timer = DawnTimeLength;
                    }
                    break;

                case TimeState.Dawn:
                    timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer <= 0) {
                        timeState = TimeState.Day;
                    }
                    break;

                case TimeState.Day:
                    break;
            }

            // TODO: call Update for all GameObjects here

            base.Update(gameTime);
        }

        /// <summary>
        /// Start Night
        /// </summary>
        public void StartNight() {
            timeState = TimeState.Night;
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
