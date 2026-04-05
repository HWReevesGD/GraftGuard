using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
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

        // Register Graft Library from JSON
        GraftLibrary.LoadLibrary(Content, "graft_library.json");

        
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
            input
        );

        /*mainMenu = new MainMenu(inputManager);
        pauseMenu = new PauseMenu(this.world, inputManager);
        towerGrafting = new TowerGraftingGUI();*/
    }

    protected override void Update(GameTime gameTime)
    {
        _gameManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.ForestGreen);

        _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

        // TODO: Add your drawing code here

        // TODO: call Draw for all GameObjects here

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
