using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Graphics.Particles;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GraftGuard.Map;
internal class World
{
    public static World CurrentWorld { get; private set; }

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

    public ParticleManager Particles { get; set; }

    // Constructor
    public World()
    {
        CurrentWorld = this;

        // These parts are just for testing, this will normally start empty
        ScatteredParts = [
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            new ScatteredPart(new Vector2(0, 0), GraftLibrary.GetRandomPart()),
            ];

        CurrentMap = EnvironmentRegistry.Map;

        Player = new Player(Vector2.Zero);
        Camera = new Camera();

        TowerManager = new TowerManager(this);
        Terrain = new Terrain(Player);
        Terrain.LoadMap(CurrentMap);
        EnemyManager = new EnemyManager(this, CurrentMap);
        ProjectileManager = new ProjectileManager();
        Garage = new Garage(CurrentMap);
        Inventory = new Inventory();
        Particles = new ParticleManager();

        Player.OnDamaged += () => {
            Random rng = new Random();

            for (int i = 0; i < 35; i++)
            {
                Vector2 position = Player.Position + new Vector2(
                    -20 + (float)rng.NextDouble() * 40,
                    -20 + (float)rng.NextDouble() * 40
                    );
                Particles.Add(
                    new Particle(Placeholders.TexturePixel)
                        .SetLifetime(0.5f, 1f)
                        .SetColor(Color.Red)
                        .SetSize(Vector2.One * 10, Vector2.Zero)
                        .SetSpeed(200f, 400f)
                        .SetAngle(-(float)Math.PI * 0.15f, -(float)Math.PI * 0.85f)
                        //.SetAngle(0, (float)Math.PI * 2)
                        .SetPosition(position)
                        .SetAcceleration(new Vector2(0, 2500))
                );
            }
        };
    }

    public void OnNewGameStarted()
    {
        Player.Setup();
        Inventory.Clear();
        TowerManager.Setup();
        ProjectileManager.Setup();
        EnemyManager.Setup(this, CurrentMap);
        Particles.Clear();
    }

    public void OnStartingDawn()
    {
        
    }

    public void OnStartingDay()
    {
        
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
                    Player.Update(gameTime, inputManager);
                }
                if (EnemyManager.Enemies.Count == 0)
                {
                    PlayerData.CurrentGame.Time = TimeState.Dawn;
                }
                break;
            case TimeState.Dawn:
                if (allowPlayerControls)
                {
                    Player.Update(gameTime, inputManager);
                }
                break;
            case TimeState.Day:
                Player.Position = Garage.Center;
                break;
        }

        
        EnemyManager.Update(gameTime, this, inputManager);
        TowerManager.Update(gameTime, this, inputManager, state);
        ProjectileManager.Update(gameTime, this, inputManager);
        Terrain.Update(gameTime);
        Garage.Update(gameTime, this);
        Particles.Update(gameTime);

        Camera.Position = Player.Position;
        Camera.Update(gameTime);

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
        EnemyManager.Draw(batch, gameTime, this, inputManager);
        ProjectileManager.Draw(batch, gameTime, this, inputManager);
        if (renderPlayer)
        {
            Player.Draw(gameTime, batch);
        }

        Particles.Draw(batch, gameTime);
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
