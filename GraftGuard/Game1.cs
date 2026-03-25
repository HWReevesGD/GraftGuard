using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard;

public class Game1 : Game
{
    public GraphicsDeviceManager Graphics;
    private SpriteBatch _spriteBatch;
    private GameManager _gameManager;

    private InputManager input;


    //private static readonly float NightTimeLength = 5;
    //private static readonly float DawnTimeLength = 10;
    //private World world;

    //private MainMenu mainMenu;
    //private PauseMenu pauseMenu;
    //private TowerGraftingGUI towerGrafting;

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        PlayerData.Load();

        input = new InputManager();
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
        PartBehaviorRegistry.Register("Flaming", PartFlaming.Create);

        // Serialize part behavior names
        PartBehaviorRegistry.Save(Content);

        // Content for Static classes
        Fonts.LoadContent(Content);
        Placeholders.LoadContent(Content);

        Projectile.LoadContent(Content);
        Player.LoadContent(Content);

        // TODO: use this.Content to load your game content here
        // Loading Tower Content
        Tower.LoadContent(Content);
        MainMenu.LoadContent(Content);
        PauseMenu.LoadContent(Content);
        GameOverScreen.LoadContent(Content);

        // Registering Towers
        TowerRegistry.Register("Spinner", TowerSpinner.Create, TowerSpinner.DrawPreview);
        TowerRegistry.Register("Trap", TowerTrap.Create, TowerTrap.DrawPreview);


        // Add Testing World
        var world = new World();

        _gameManager = new GameManager(
            world,
            new MainMenu(input),
            new PauseMenu(world, input),
            new GameOverScreen(world),
            new TowerGraftingGUI(),
            input
        );

        /*mainMenu = new MainMenu(inputManager);
        pauseMenu = new PauseMenu(this.world, inputManager);
        towerGrafting = new TowerGraftingGUI();*/
    }

    protected override void Update(GameTime gameTime)
    {
        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //    Exit();

        //Yuxuan what you're looking for is now in GameManager. You can move specifics around but I'd like to keep Game1 as streamlined as possible.
        //In theory we never have to touch this file again. I kept your old code here commented out

        /*switch (gameState)
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
                        towerGrafting.Update(gameTime, inputManager, _world);
                        break;
                }
                break;
        }*/

        _gameManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.ForestGreen);

        _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

        // TODO: Add your drawing code here

        // TODO: call Draw for all GameObjects here
        /*
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

                // Draw Tower Grafting GUI
                if (timeState == TimeState.Day)
                {
                    towerGrafting.Draw(_spriteBatch, gameTime);
                }

                // Mouse Debug
                _spriteBatch.DrawString(Fonts.Arial, $"Mouse Screen: {Mouse.GetState().Position.ToVector2()}\nMouse World: {Vector2.Transform(Mouse.GetState().Position.ToVector2(), _world.Camera.ScreenToWorld)}", new Vector2(64, 128), Color.White);

                _spriteBatch.DrawString(
                    Fonts.Arial,
                    $"GAME\n" +
                    $"STATE: {timeState}\n" +
                    $"TIMER: {timer}\n",
                    new Vector2(64, 0),
                    Color.White
                );

                break;
        }*/

        _gameManager.Draw(_spriteBatch, gameTime);

        _spriteBatch.End(); 
        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        PlayerData.Save();
        base.OnExiting(sender, args);
    }
}
