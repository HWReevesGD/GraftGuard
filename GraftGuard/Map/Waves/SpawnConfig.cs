using GraftGuard.Map.Enemies;
using Microsoft.Xna.Framework;
using System;

namespace GraftGuard.Map.Waves;

internal class SpawnConfig
{
    public Action<Vector2, EnemyManager> Construct { get; init; }
    public int Count { get; init; }
    public SpawnConfig(Action<Vector2, EnemyManager> enemyConstructor, int count)
    {
        Construct = enemyConstructor;
        Count = count;
    }
}