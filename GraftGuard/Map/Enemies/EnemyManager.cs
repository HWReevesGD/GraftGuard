using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies;
internal class EnemyManager
{
    private List<PathNode> _pathNodes;
    public List<Enemy> Enemies;


    public EnemyManager()
    {

        _pathNodes = [];
        Enemies = [
            new EnemyDummy(new Vector2(400, 350), BaseRegistry.GetRandom()),
            ];
        
    }

    public void Update(GameTime time, InputManager inputManager)
    {
        for (int index = 0; index < Enemies.Count; index++)
        {
            Enemy enemy = Enemies[index];
            enemy.Update(time, inputManager);

            if (enemy.Health <= 0.0f)
            {
                enemy.OnDeath();
                Enemies.RemoveAt(index);
                index--;
            }
        }
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.Draw(time, batch);
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
