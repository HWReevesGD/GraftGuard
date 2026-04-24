using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Pathing;
using GraftGuard.Map.Waves;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GraftGuard.UI.Screens;

/// <summary>
/// A world for background visual purposes that has random towers and random enemies that cannot lose
/// </summary>
internal class MainMenuBackgroundWorld
{
    private readonly static float cameraPanSpeed = 0.5f;

    // percentage range for path length from a spawn to the garage
    // (min 0.2f = spawn minimum 20% of the way from a spawn to the garage)
    private readonly static float towerSpawnPathLengthMinPercentage = 0.25f;
    private readonly static float towerSpawnPathLengthMaxPercentage = 0.75f;
    private readonly static float towerSpawnMaxRandomOffset = 75f;
    private readonly static int amountOfTowers = 15;

    private static readonly float minEnemySpawnPeriod = 1f;
    private static readonly int maxEnemyCount = 50;
    private static readonly float timeScale = 0.8f;

    // automatic camera panning cycle stuff
    private static readonly float xCycleOffset = 150;
    private static readonly float yCycleOffset = -300;
    private static readonly float xCycleScale = 0.25f;
    private static readonly float yCycleScale = 0.25f;
    private static readonly float manualCameraPanSpeed = 500;

    private readonly World world;
    private readonly InputManager idleInputManager; // input manager that will never update, because it's for the background (world needs it)
    private readonly InputManager inputManager;
    private readonly TimeState timeState;
    private readonly Random rng;

    private bool manualPanningEnabled;
    private float x;
    private float y;
    private float lastEnemySpawnTime;

    /// <summary>
    /// Instanitate background world
    /// </summary>
    /// <param name="inputManager">InputManager from GameManager</param>
    /// <param name="drawing">DrawManager</param>
    public MainMenuBackgroundWorld(InputManager inputManager, DrawManager drawing)
    {
        this.world = new World(drawing);
        this.idleInputManager = new InputManager();
        this.inputManager = inputManager;
        this.timeState = TimeState.Night; // always night

        rng = new Random();

        // move player into the garage so player-targetting enemies go to it
        world.Player.Position = world.Garage.Center;
        world.ShowTowerDecay = false;

        CreateTowers(amountOfTowers);
    }

    /// <summary>
    /// Get a random point where a tower could be spawned
    /// </summary>
    /// <returns>Point</returns>
    private Vector2 GetRandomTowerSpawnPosition()
    {
        Vector2 enemySpawn = EnvironmentRegistry.Map.EnemySpawns[rng.Next(0, EnvironmentRegistry.Map.EnemySpawns.Count)];

        List<PathNode> path = world.EnemyManager.PathManager.FindPath(
                world,
                enemySpawn,
                new PathManager.PathSettings() { Goal = PathManager.PathGoal.Garage }
                );

        float pathSpawnDistancePercentage = MathHelper.Lerp(
            towerSpawnPathLengthMinPercentage,
            towerSpawnPathLengthMaxPercentage,
            rng.NextSingle()
            );

        Vector2 nodePosition = path[(int)(path.Count * pathSpawnDistancePercentage)].WorldPosition;
        return new Vector2(
            nodePosition.X - towerSpawnMaxRandomOffset / 2 + rng.NextSingle() * towerSpawnMaxRandomOffset,
            nodePosition.Y - towerSpawnMaxRandomOffset / 2 + rng.NextSingle() * towerSpawnMaxRandomOffset
            );
    }

    /// <summary>
    /// Create an amount of random towers positioned on existing paths from enemy spawns to the garage
    /// </summary>
    /// <param name="amount">Amount of towers to spawn</param>
    private void CreateTowers(int amount)
    {
        // create random towers for the world

        for (int _ = 0; _ < amount; _++)
        {
            // pick random tower type and random parts
            TowerDefinition tower = TowerRegistry.Towers[rng.Next(0, TowerRegistry.Towers.Count)];
            List<PartDefinition> parts = new List<PartDefinition>();
            Vector2 position = GetRandomTowerSpawnPosition();

            // at least one part
            for (int i = 0; i < rng.Next(1, 4); i++)
                parts.Add(GraftLibrary.GetRandomPart());

            // MakeTower() will update path costs, so there will automatically not be
            // towers overlapping on the main menu
            world.TowerManager.MakeTower(tower, position, parts);
        }
    }

    /// <summary>
    /// Automatic panning around the map
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    private void UpdateCameraAutoPanning(GameTime gameTime)
    {
        MapDefinition map = EnvironmentRegistry.Map;
        Rectangle pathingArea = map.PathingArea;
        int width = pathingArea.Width;
        int height = pathingArea.Height;

        float cycleWidth = width * xCycleScale;
        float cycleHeight = height * yCycleScale;

        double xCycle = Math.Cos(gameTime.TotalGameTime.TotalSeconds / 3 * cameraPanSpeed);
        double yCycle = Math.Sin(gameTime.TotalGameTime.TotalSeconds * cameraPanSpeed);

        x = (float)(xCycle * cycleWidth / 2 + pathingArea.Center.X) + xCycleOffset;
        y = (float)(yCycle * cycleHeight / 2 + pathingArea.Center.Y) + yCycleOffset;
    }

    /// <summary>
    /// Update camera panning controlled by WASD
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void UpdateCameraManualPanning(GameTime gameTime)
    {
        float delta = gameTime.Delta();
        if (inputManager.IsKeyDown(Keys.W))
            y -= manualCameraPanSpeed * delta;
        if (inputManager.IsKeyDown(Keys.A))
            x -= manualCameraPanSpeed * delta;
        if (inputManager.IsKeyDown(Keys.S))
            y += manualCameraPanSpeed * delta;
        if (inputManager.IsKeyDown(Keys.D))
            x += manualCameraPanSpeed * delta;
    }

    /// <summary>
    /// Update background world simulation and pan the camera
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void Update(GameTime gameTime)
    {
        // update world

        // spawn enemies in interval if the amount is below the maximum
        if (gameTime.TotalGameTime.TotalSeconds - lastEnemySpawnTime > minEnemySpawnPeriod)
        {
            lastEnemySpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;
            if (world.EnemyManager.Enemies.Count < maxEnemyCount)
            {
                // yes it is just spawning the wave for round 0 that is intentional
                NightWaveSet waveSet = WavesRegistry.GetRandomForRound(0);
                foreach (NightWave wave in waveSet.Waves)
                    world.EnemyManager.SpawnWave(wave);
            }
        }

        world.Update(gameTime.Scale(timeScale), idleInputManager, timeState, false);

        // remove enemies that wouldve killed the player via garage
        for (int i = 0; i < world.EnemyManager.Enemies.Count; i++)
        {
            Enemy enemy = world.EnemyManager.Enemies[i];
            if (!enemy.IsDead && enemy.Hitbox.Intersects(world.Garage.GameOverBounds))
            {
                world.EnemyManager.Enemies.RemoveAt(i);
                i--;
            }
        }

        // camera

        if (manualPanningEnabled)
            UpdateCameraManualPanning(gameTime);
        else
            UpdateCameraAutoPanning(gameTime);

        world.Camera.Position = new Vector2(x, y);

        // debug keys

        // spawn wave of enemies
        if (inputManager.WasKeyPressStarted(Keys.Y))
        {
            NightWaveSet waveSet = WavesRegistry.GetRandomForRound(1);
            foreach (NightWave wave in waveSet.Waves)
                world.EnemyManager.SpawnWave(wave);
        }

        // make a tower
        if (inputManager.WasKeyPressStarted(Keys.U))
            CreateTowers(1);

        // toggle manual camera panning
        if (inputManager.WasKeyPressStarted(Keys.P))
            manualPanningEnabled = !manualPanningEnabled;
    }

    /// <summary>
    /// Draw the world
    /// </summary>
    /// <param name="drawing">DrawManager</param>
    /// <param name="gameTime">GameTime</param>
    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        drawing.ExtraMatrix = world.Camera.WorldToScreen * Matrix.CreateScale(0.25f);
        // Draw by the Camera's Position
        world.DrawCamera(drawing, gameTime.Scale(timeScale), timeState, inputManager, false);
        drawing.ExtraMatrix = null;
    }
}
