using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.UI;
using GraftGuard.UI.Grafting;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
        public GraphicsDeviceManager Graphics;
        private SpriteBatch _spriteBatch;

        private GameState gameState;
        private TimeState timeState;
        private float timer;

        private static readonly float NightTimeLength = 5;
        private static readonly float DawnTimeLength = 5;
        private InputManager inputManager;
        private World _testingWorld;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.gameState = GameState.Game;
            this.timeState = TimeState.Night;
            
            inputManager = new InputManager();
            Interface.Initialize(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Content for Static classes
            Fonts.LoadContent(Content);
            Placeholders.LoadContent(Content);

            Player.LoadContent(Content);

            // TODO: use this.Content to load your game content here
            // Loading Tower Content
            Tower.LoadContent(Content);
            PartDefinition.LoadContent(Content);

            // Setting Tower Registry
            TowerRegistry.Register("Spinner", TowerSpinner.Create);
            TowerRegistry.Register("Trap", TowerTrap.Create);

            // Add Testing World
            _testingWorld = new World();

            // Testing Towers (Testing only!)
            TowerManager t = _testingWorld.TowerManager;
            t.AddTower(new TowerTrap(new Vector2(200, 200)));
            t.AddTower(new TowerSpinner(new Vector2(400, 200)));
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

                    _testingWorld.Update(gameTime, inputManager);

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

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // TODO: Add your drawing code here

            // TODO: call Draw for all GameObjects here

            switch (gameState)
            {
                case GameState.MainMenu:
                    _spriteBatch.DrawString(Fonts.Arial, "MAIN MENU", Vector2.Zero, Color.White);
                    break;

                case GameState.Paused:
                    _spriteBatch.DrawString(Fonts.Arial, "PAUSED", Vector2.Zero, Color.White);
                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(Fonts.Arial, "GAME OVER", Vector2.Zero, Color.White);
                    break;

                case GameState.Game:
                    // Testing Stuff
                    _testingWorld.Draw(_spriteBatch, gameTime);
                    
                    _spriteBatch.DrawString(
                        Fonts.Arial,
                        $"GAME\n" +
                        $"STATE: {timeState}\n" +
                        $"TIMER: {timer}\n",
                        new Vector2(64, 0),
                        Color.White
                    );
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
