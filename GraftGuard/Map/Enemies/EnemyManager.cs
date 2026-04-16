using GraftGuard.Map.Pathing;
﻿using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using GraftGuard.Utility;
using GraftGuard.Grafting;
using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Map.Waves;
using System;
using System.Diagnostics;

namespace GraftGuard.Map.Enemies;
internal class EnemyManager
{
    public List<Enemy> Enemies { get; set; }
    public PathManager PathManager { get; set; }
    public List<Vector2> SpawnLocations { get; set; }
    public EnemyManager(World world, MapDefinition map)
    {
        Setup(world, map);
        WaveManager = new WaveManager();
        WaveManager.SpawnWave += SpawnWave;
    }

    public WaveManager WaveManager; 

    /// <summary>
    /// Sets up the <see cref="EnemyManager"/> for a new Session
    /// </summary>
    public void Setup(World world, MapDefinition map)
    {
        PathManager = new PathManager();
        Rectangle pathingArea = map.PathingArea;
        PathManager.Start = pathingArea.Location.ToVector();
        PathManager.End = (pathingArea.Location + pathingArea.Size).ToVector();
        SpawnLocations = map.EnemySpawns.ToList();

        PathManager.BuildGrid(world);

        Enemies = [];
    }

    public void BeginNight()
    {
        WaveManager.StartWaves(WavesRegistry.GetRandomForRound(PlayerData.CurrentGame.GameLog.RoundsSurvived));
    }

    public void SpawnWave(NightWave wave)
    {
        Debug.WriteLine("Spawning Wave...");
        int spawnLocationIndex = 0;
        foreach (SpawnConfig spawnConfig in wave.Spawns)
        {
            for (int index = 0; index < spawnConfig.Count; index++)
            {
                Enemy enemy = spawnConfig.Construct(SpawnLocations[spawnLocationIndex++], this);
                Enemies.Add(enemy);
                if (spawnLocationIndex >= SpawnLocations.Count)
                {
                    spawnLocationIndex = 0;
                }
            }
        }
    }

    private List<PathNode> _debugPath;
    public void Update(GameTime time, World world, InputManager inputManager)
    {
        // Update Waves
        WaveManager.Update(PlayerData.CurrentGame.Timer);

        // Updated Enemies
        for (int index = 0; index < Enemies.Count; index++)
        {
            Enemy enemy = Enemies[index];
            enemy.Update(time, inputManager, world, PathManager);

            // Check if the enemy just died this frame
            if (enemy.Health <= 0.0f && !enemy.IsDead)
            {
                enemy.OnDeath(); 
            }

            // Only remove if it's dead AND all visual effects (parts) are done
            if (enemy.IsDead && enemy.Visual.AllPartsLanded)
            {
                Enemies.RemoveAt(index);
                index--;
            }
        }
    }

    public void Draw(DrawManager drawing, GameTime time, World world, InputManager inputManager)
    {
        PathManager.Draw(drawing, time);
        foreach (Enemy enemy in Enemies)
        {
            enemy.Draw(time, drawing, inputManager, world);
        }
    }

    /// <summary>
    /// Returns a <see cref="List"/> of <see cref="Enemy"/> with all enemies that overlap any of the given <see cref="Rectangle"/>s or <see cref="Circle"/>s
    /// </summary>
    /// <param name="boxes">List of <see cref="Rectangle"/>s to check</param>
    /// <param name="circles">List of <see cref="Circle"/>s to check</param>
    public List<Enemy> GetEnemiesInAreas(List<Rectangle> boxes, List<Circle> circles)
    {
        List<Enemy> areaEnemies = [];
        foreach (Enemy enemy in Enemies)
        {
            if (boxes.Any((box) => box.Intersects(enemy.Hitbox)) || circles.Any((circle) => circle.Intersects(enemy.Hitbox)))
            {
                areaEnemies.Add(enemy);
            }
        }
        return areaEnemies;
    }

    public void DealDamageInAreas(List<Rectangle> boxes, List<Circle> circles, Damage damage)
    {
        List<Enemy> enemies = GetEnemiesInAreas(boxes, circles);
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(damage);
        }
    }
}
