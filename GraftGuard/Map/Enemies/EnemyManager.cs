using GraftGuard.Map.Pathing;
﻿using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using GraftGuard.Utility;
using GraftGuard.Grafting;
using GraftGuard.Data;

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

        Enemies = [
            new EnemyDummy(new Vector2(0, 0), GraftLibrary.GetRandomBase()),
            ];
    }

    public void BeginNight()
    {
        foreach(Vector2 spawn in Spawns)
        {
            for(int i = 0; i <= (int) PlayerData.CurrentGame.CurrentDifficulty; i++)
            {
                Enemies.Add(new EnemyDummy(spawn, GraftLibrary.GetRandomBase()));
            }
        }
    }

    private List<PathNode> _debugPath;
    public void Update(GameTime time, World world, InputManager inputManager)
    {
        _debugPath = PathManager.FindPath(new Point(1, 1), ((inputManager.MouseWorldPosition.ToVector() - PathManager.Start) / (PathNode.GridDistance)).ToPoint(), false);
        for (int index = 0; index < Enemies.Count; index++)
        {
            Enemy enemy = Enemies[index];
            enemy.Update(time, inputManager);

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

    public void Draw(SpriteBatch batch, GameTime time)
    {
        PathManager.Draw(batch, time);
        foreach (Enemy enemy in Enemies)
        {
            enemy.Draw(time, batch);
        }

        foreach (PathNode node in _debugPath ?? [])
        {
            // TEMP: This is to stop a crash that was happening, I don't know how this code works yet - Harrison L
            if (node is null)
            {
                continue;
            }
            batch.DrawCircle(node.CheckCircle, Color.Green);
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

    public void DealDamageInAreas(List<Rectangle> boxes, List<Circle> circles, float damage)
    {
        List<Enemy> enemies = GetEnemiesInAreas(boxes, circles);
        foreach (Enemy enemy in enemies)
        {
            enemy.Health -= damage;
        }
    }
}
