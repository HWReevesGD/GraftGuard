using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Map.Enemies;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GraftGuard.Map;
internal class World
{
    // Properties
    public Player Player { get; set; }
    public EnemyManager EnemyManager { get; set; }
    public TowerManager TowerManager { get; set; }
    public TowerGrafter TowerGrafter { get; set; }
    public Terrain Terrain { get; set; }
    public Camera Camera { get; set; }
    public Garage Garage { get; set; }
    public List<ScatteredPart> ScatteredParts { get; set; }

    private static Texture2D torsoTex;
    private static Texture2D headTex;

    public static void LoadContent(ContentManager content) {
        torsoTex = content.Load<Texture2D>("Parts/enemy_0");
        headTex = content.Load<Texture2D>("Parts/enemy_5");
    }

    // Constructor
    public World()
    {

        // These parts are just for testing, this will normally start empty
        ScatteredParts = [
            new ScatteredPart(new Vector2(420, 220), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(420, 320), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(520, 320), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(620, 320), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(720, 320), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(420, 220), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(420, 220), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(520, 220), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(620, 220), PartRegistry.GetRandom()),
            new ScatteredPart(new Vector2(720, 220), PartRegistry.GetRandom()),
            ];

        TowerManager = new TowerManager(this);
        TowerGrafter = new TowerGrafter(TowerManager);
        Terrain = new Terrain();
        EnemyManager = new EnemyManager(this);
        Garage = new Garage();

        Player = new Player(Vector2.Zero);
        Camera = new Camera();
    }

    // Methods
    public void Update(GameTime gameTime, InputManager inputManager, TimeState state)
    {
        switch (state)
        {
            case TimeState.Night:
                Player.Update(gameTime, inputManager, this);
                break;
            case TimeState.Dawn:
                Player.Update(gameTime, inputManager, this);
                break;
            case TimeState.Day:
                Player.Position = Garage.Center;
                Camera.UpdateFreeMovement(gameTime, inputManager);
                TowerGrafter.Update(gameTime, inputManager, this);
                break;
        }

        inputManager.Update(Camera);
        EnemyManager.Update(gameTime, this, inputManager);
        TowerManager.Update(gameTime, this, inputManager, state);
        Terrain.Update(gameTime);
        Garage.Update(gameTime, this);
    }

    public void DrawStatic(SpriteBatch batch, GameTime gameTime, TimeState state, InputManager inputManager)
    {
        batch.DrawString(Fonts.Arial, $"Mouse Screen: {Mouse.GetState().Position.ToVector2()}\nMouse World: {Vector2.Transform(Mouse.GetState().Position.ToVector2(), Camera.ScreenToWorld)}", new Vector2(64, 128), Color.White);
        if (state == TimeState.Day)
        {
            TowerGrafter.Draw(batch, gameTime);
        }
    }

    public void DrawCamera(SpriteBatch batch, GameTime gameTime, TimeState state, InputManager inputManager)
    {
        Terrain.Draw(batch, gameTime);

        foreach (ScatteredPart part in ScatteredParts)
        {
            part.Draw(gameTime, batch);
        }

        Garage.Draw(batch, gameTime);

        TowerManager.Draw(batch, gameTime, this, inputManager, state);
        EnemyManager.Draw(batch, gameTime);
        Player.Draw(gameTime, batch);
    }
    public void UpdatePaths()
    {
        EnemyManager.PathManager.BuildGrid(this);
    }
}
