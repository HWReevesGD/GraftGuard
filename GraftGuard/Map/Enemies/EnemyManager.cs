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

namespace GraftGuard.Map.Enemies;
internal class EnemyManager
{
    public List<Enemy> Enemies;
    public PathManager PathManager { get; set; }
    public List<Vector2> Spawns { get; set; }
    public EnemyManager(World world, MapDefinition map) => Setup(world, map);

    /// <summary>
    /// Sets up the <see cref="EnemyManager"/> for a new Session
    /// </summary>
    public void Setup(World world, MapDefinition map)
    {
        PathManager = new PathManager();
        Rectangle pathingArea = map.PathingArea;
        PathManager.Start = pathingArea.Location.ToVector();
        PathManager.End = (pathingArea.Location + pathingArea.Size).ToVector();
        Spawns = map.EnemySpawns.ToList();

        PathManager.BuildGrid(world);

        Enemies = [];
    }

    public void BeginNight()
    {
        int spawnIndex = -1;
        foreach(Vector2 spawn in Spawns)
        {
            spawnIndex++;
            int difficulty = (int)PlayerData.CurrentGame.CurrentDifficulty;
            for (int i = 0; i <= difficulty; i++)
            {
                Enemies.Add(new EnemyHumanoid(spawn));
                if (spawnIndex % 2 == 0 && difficulty > 1)
                {
                    Enemies.Add(new EnemyCentipede(spawn, this));
                }
            }
        }
    }

    private List<PathNode> _debugPath;
    public void Update(GameTime time, World world, InputManager inputManager)
    {
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
