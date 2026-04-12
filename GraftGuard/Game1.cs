using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.UI;
using GraftGuard.UI.Screens;
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

    private RenderTarget2D _renderTarget;
    private readonly int _virtualWidth = 1920;  
    private readonly int _virtualHeight = 1080; 

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Graphics.PreferredBackBufferWidth = 1920;
        Graphics.PreferredBackBufferHeight = 1080;

        Graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        PlayerData.Load();

        input = new InputManager();
        Interface.Initialize(this);

        _renderTarget = new RenderTarget2D(GraphicsDevice, _virtualWidth, _virtualHeight);

        Window.AllowUserResizing = true;

        base.Initialize();
    }

    public Matrix GetScaleMatrix()
    {
        float scaleX = (float)GraphicsDevice.Viewport.Width / _virtualWidth;
        float scaleY = (float)GraphicsDevice.Viewport.Height / _virtualHeight;
        return Matrix.CreateScale(scaleX, scaleY, 1.0f);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Register Graft Library from JSON
        GraftLibrary.LoadLibrary(Content, "graft_library.json");

        
        // Register (and save) part behaviors
        PartBehaviorRegistry.Register("Slashing", PartSlashing.Create);
        PartBehaviorRegistry.Register("Flaming", PartFlaming.Create);
        PartBehaviorRegistry.Register("Zapping", PartZapping.Create);
        PartBehaviorRegistry.Register("Shotgunning", PartShotgunning.Create);

        // Serialize part behavior names
        PartBehaviorRegistry.Save(Content);

        // Content for Static classes
        Fonts.LoadContent(Content);
        Placeholders.LoadContent(Content);

        Projectile.LoadContent(Content);
        Player.LoadContent(Content);
        Enemy.LoadContent(Content);

        // Import and Load the Environment
        EnvironmentRegistry.LoadContent(Content);

        // TODO: use this.Content to load your game content here
        // Loading Tower Content
        Tower.LoadContent(Content);
        MainMenu.LoadContent(Content);
        PauseMenu.LoadContent(Content);
        GameOverScreen.LoadContent(Content);
        GameHUD.LoadContent(Content);
        
        EnvironmentProps.LoadContent(Content);

        // Registering Towers
        TowerRegistry.Register("Spinner", TowerSpinner.Create, TowerSpinner.DrawPreview, Tower.TexturePlaceholderTower);
        TowerRegistry.Register("Trap", TowerTrap.Create, TowerTrap.DrawPreview, Tower.TexturePlaceholderGround);
        TowerRegistry.Register("Turret", TowerTurret.Create, TowerTurret.DrawPreview, Tower.TTurret);


        // Add Testing World
        var world = new World();

        _gameManager = new GameManager(
            world,
            new MainMenu(this, input),
            new PauseMenu(world, input),
            new GameOverScreen(world),
            new GameHUD(),
            new TowerGraftingGUI(),
            new NightPlacementGUI(),
            input,
            _spriteBatch
        );

        /*mainMenu = new MainMenu(inputManager);
        pauseMenu = new PauseMenu(this.world, inputManager);
        towerGrafting = new TowerGraftingGUI();*/
    }

    protected override void Update(GameTime gameTime)
    {
        input.ResolutionScaleMatrix = GetScaleMatrix();
        _gameManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //GraphicsDevice.Clear(ClearOptions.Target, Color.ForestGreen, -100, 0);
        
        GraphicsDevice.SetRenderTarget(_renderTarget);
        GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target | ClearOptions.Stencil, Color.ForestGreen, 0.0f, 0);

        _gameManager.Draw(gameTime);

        GraphicsDevice.SetRenderTarget(null); 
        GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target | ClearOptions.Stencil, Color.Black, 0.0f, 0);    

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: GetScaleMatrix());
        _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        PlayerData.Save();
        base.OnExiting(sender, args);
    }
}
