using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GraftGuard.Map;
internal class World
{
    // Properties
    public Player Player { get; set; }
    public EnemyManager EnemyManager { get; set; }
    public TowerManager TowerManager { get; set; }
    public ProjectileManager ProjectileManager { get; set; }
    public Inventory Inventory { get; set; }
    public Terrain Terrain { get; set; }
    public Camera Camera { get; set; }
    public Garage Garage { get; set; }
    public static List<ScatteredPart> ScatteredParts { get; set; }
    public MapDefinition CurrentMap { get; set; }

    // Constructor
    public World()
    {

        // These parts are just for testing, this will normally start empty
        ScatteredParts = [
            new ScatteredPart(new Vector2(420, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(420, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(520, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(620, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(720, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(420, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(420, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(520, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(620, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(720, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(420, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(420, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(520, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(620, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(720, 320), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(420, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(420, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(520, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(620, 220), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(720, 220), GraftLibrary.GetRandomPart()),
            ];

        CurrentMap = EnvironmentRegistry.Map;

        TowerManager = new TowerManager(this);
        Terrain = new Terrain();
        Terrain.LoadMap(CurrentMap);
        EnemyManager = new EnemyManager(this, CurrentMap);
        ProjectileManager = new ProjectileManager();
        Garage = new Garage(CurrentMap);
        Inventory = new Inventory();

        Player = new Player(Vector2.Zero);
        Camera = new Camera();
    }

    public void OnNewGameStarted()
    {
        Player.Setup();
        Inventory.Clear();
        TowerManager.Setup();
        ProjectileManager.Setup();
        EnemyManager.Setup(this, CurrentMap);
    }

    public void OnStartingDawn()
    {
        
    }

    public void OnStartingDay()
    {
        Player.ClearHeldParts();
        ScatteredParts = [];
    }

    public void OnStartingNight()
    {
        EnemyManager.BeginNight();
    }

    // Methods
    public void Update(GameTime gameTime, InputManager inputManager, TimeState state, bool allowPlayerControls)
    {
        switch (state)
        {
            case TimeState.Night:
                if (allowPlayerControls)
                {
                    Player.Update(gameTime, inputManager, this);
                }
                break;
            case TimeState.Dawn:
                if (allowPlayerControls)
                {
                    Player.Update(gameTime, inputManager, this);
                }
                break;
            case TimeState.Day:
                Player.Position = Garage.Center;
                break;
        }

        Camera.Update(gameTime);
        EnemyManager.Update(gameTime, this, inputManager);
        TowerManager.Update(gameTime, this, inputManager, state);
        ProjectileManager.Update(gameTime, this, inputManager);
        Terrain.Update(gameTime);
        Garage.Update(gameTime, this);
    }

    public void DrawCamera(SpriteBatch batch, GameTime gameTime, TimeState state, InputManager inputManager, bool renderPlayer)
    {
        Terrain.Draw(batch, gameTime);

        foreach (ScatteredPart part in ScatteredParts)
        {
            part.Draw(gameTime, batch);
        }

        Garage.Draw(batch, gameTime);

        TowerManager.Draw(batch, gameTime, this, inputManager, state);
        EnemyManager.Draw(batch, gameTime);
        ProjectileManager.Draw(batch, gameTime, this, inputManager);
        if (renderPlayer)
        {
            Player.Draw(gameTime, batch);
        }
    }

    public static void ScatterPart(Vector2 location, PartDefinition part)
    {
        ScatteredParts.Add(new ScatteredPart(location, part));
    }
    public void UpdatePaths()
    {
        EnemyManager.PathManager.BuildGrid(this);
    }
}
