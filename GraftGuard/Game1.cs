using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace GraftGuard;
enum GameState
{
    MainMenu,
    Paused,
    GameOver,
    Game
}

enum TimeState
{
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
    private static readonly float DawnTimeLength = 10;
    private InputManager inputManager;
    private World _world;

    private MainMenu mainMenu;
    private PauseMenu pauseMenu;

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        this.gameState = GameState.MainMenu;
        this.timeState = TimeState.Night;

        inputManager = new InputManager();
        Interface.Initialize(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Register parts from JSON
        PartRegistry.LoadFromLibrary(Content, "graft_library.json");

        // Register bases from JSON
        BaseRegistry.LoadFromLibrary(Content, "graft_library.json");

        // Register (and save) part behaviors
        PartBehaviorRegistry.Register("Slashing", PartSlashing.Create);

        // Serialize part behavior names
        PartBehaviorRegistry.Save(Content);

        // Content for Static classes
        Fonts.LoadContent(Content);
        Placeholders.LoadContent(Content);

        Player.LoadContent(Content);

        // TODO: use this.Content to load your game content here
        // Loading Tower Content
        Tower.LoadContent(Content);
        MainMenu.LoadContent(Content);
        PauseMenu.LoadContent(Content);
        World.LoadContent(Content);

        // Registering Towers
        TowerRegistry.Register("Spinner", TowerSpinner.Create, TowerSpinner.DrawPreview);
        TowerRegistry.Register("Trap", TowerTrap.Create, TowerTrap.DrawPreview);


        // Add Testing World
        _world = new World();

        mainMenu = new MainMenu(inputManager);
        pauseMenu = new PauseMenu(_world, inputManager);
    }

    protected override void Update(GameTime gameTime)
    {
        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //    Exit();

        switch (gameState)
        {
            case GameState.MainMenu:
                inputManager.Update();
                if (inputManager.WasKeyPressStarted(Keys.Escape))
                {
                    Exit();
                    break;
                }
                else if (inputManager.WasKeyPressStarted(Keys.Enter))
                {
                    gameState = GameState.Game;
                    timeState = TimeState.Dawn;
                    timer = DawnTimeLength;
                }
                mainMenu.Update(gameTime);
                break;

            case GameState.Paused:
                inputManager.Update();
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
                inputManager.Update();
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

                _world.Update(gameTime, inputManager, timeState, true);

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
    public void StartNight()
    {
        timeState = TimeState.Night;
        timer = NightTimeLength;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

        // TODO: Add your drawing code here

        // TODO: call Draw for all GameObjects here

        switch (gameState)
        {
            case GameState.MainMenu:
                //_spriteBatch.DrawString(Fonts.Arial, "MAIN MENU", Vector2.Zero, Color.White);
                mainMenu.Draw(_spriteBatch, gameTime);
                break;

            case GameState.Paused:
                pauseMenu.Draw(_spriteBatch, gameTime, timeState);
                break;

            case GameState.GameOver:
                _spriteBatch.DrawString(Fonts.Arial, "GAME OVER", Vector2.Zero, Color.White);
                break;

            case GameState.Game:

                _spriteBatch.End();

                // Draw by the Camera's Position
                _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _world.Camera.WorldToScreen);

                _world.DrawCamera(_spriteBatch, gameTime, timeState, inputManager, true);

                _spriteBatch.End();

                // Drawn on the Screen Directly
                _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                _world.DrawStatic(_spriteBatch, gameTime, timeState, inputManager);

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
