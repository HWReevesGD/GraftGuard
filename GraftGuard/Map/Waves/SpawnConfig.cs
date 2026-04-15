using GraftGuard.Map.Enemies;
using Microsoft.Xna.Framework;
using System;

namespace GraftGuard.Map.Waves;

internal class SpawnConfig
{
    public Action<Vector2, EnemyManager> Construct { get; set; }
    public int Count { get; set; }
    public SpawnConfig(Action<Vector2, EnemyManager> enemyConstructor, int count)
    {
        Construct = enemyConstructor;
        Count = count;
    }
}