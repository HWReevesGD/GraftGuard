using GraftGuard.Grafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard
{
    enum GameState
    {
        MainMenu,
        Paused,
        GameOver,
        Game
    }

    enum TimeState {
        Night,
        Dawn,
        Day
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameState gameState;
        private TimeState timeState;
        private float timer;

        private SpriteFont arial;

        private static readonly float NightTimeLength = 5;
        private static readonly float DawnTimeLength = 5;
        private InputManager inputManager;
        private World _testingWorld;

        private Player player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.gameState = GameState.Game;
            this.timeState = TimeState.Night;
            inputManager = new InputManager();

            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            this.arial = Content.Load<SpriteFont>("arial");

            Texture2D playerTexture = Content.Load<Texture2D>("playerplaceholder");
            this.player = new Player(Vector2.Zero, new Vector2(50, 50), playerTexture);

            // TODO: use this.Content to load your game content here
            // Loading Tower Content
            Tower.LoadContent(Content);

            // Add Testing World
            _testingWorld = new World();
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            inputManager.Update();

            switch (gameState)
            {
                case GameState.MainMenu:
                    if (inputManager.WasKeyPressStarted(Keys.Escape))
                    {
                        Exit();
                        break;
                    }
                    else if (inputManager.WasKeyPressStarted(Keys.Enter))
                    {
                        gameState = GameState.Game;
                    }
                    break;

                case GameState.Paused:
                    if (inputManager.WasKeyPressStarted(Keys.Escape))
                    {
                        gameState = GameState.MainMenu;
                        break;
                    }
                    else if (inputManager.WasKeyPressStarted(Keys.Enter))
                    {
                        gameState = GameState.Game;
                    }
                    break;

                case GameState.GameOver:
                    if (inputManager.WasKeyPressStarted(Keys.Escape))
                    {
                        gameState = GameState.MainMenu;
                        break;
                    }
                    break;

                case GameState.Game:
                    // TODO: handle gameplay inputs here

                    if (inputManager.WasKeyPressStarted(Keys.Escape))
                    {
                        gameState = GameState.Paused;
                        break;
                    }

                    player.Update(gameTime, inputManager);

                    // handle game timers

                    switch (timeState)
                    {
                        case TimeState.Night:
                            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (timer <= 0)
                            {
                                timeState = TimeState.Dawn;
                                timer = DawnTimeLength;
                            }
                            break;

                        case TimeState.Dawn:
                            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (timer <= 0)
                            {
                                timeState = TimeState.Day;
                            }
                            break;

                        case TimeState.Day:
                            break;
                    }
                    break;
            }

            // TODO: call Update for all GameObjects here
            _testingWorld.Update(gameTime);

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

            _spriteBatch.Begin();

            // TODO: Add your drawing code here

            // TODO: call Draw for all GameObjects here

            switch (gameState)
            {
                case GameState.MainMenu:
                    _spriteBatch.DrawString(arial, "MAIN MENU", Vector2.Zero, Color.White);
                    break;

                case GameState.Paused:
                    _spriteBatch.DrawString(arial, "PAUSED", Vector2.Zero, Color.White);
                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(arial, "GAME OVER", Vector2.Zero, Color.White);
                    break;

                case GameState.Game:
                    this.player.Draw(gameTime, _spriteBatch);
                    _spriteBatch.DrawString(
                        arial,
                        $"GAME\n" +
                        $"STATE: {timeState}\n" +
                        $"TIMER: {timer}\n" +
                        $"PLAYER POSITION: {player.Position}\n" +
                        $"MOVE DIR: {inputManager.GetMovementDirection()}",
                        Vector2.Zero,
                        Color.White
                    );
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
