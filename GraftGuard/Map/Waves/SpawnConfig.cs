using GraftGuard.Map.Enemies;
using Microsoft.Xna.Framework;
using System;

namespace GraftGuard.Map.Waves;

internal class SpawnConfig
{
    public Func<Vector2, EnemyManager, Enemy> Construct { get; init; }
    public int Count { get; init; }
    public SpawnConfig(Func<Vector2, EnemyManager, Enemy> enemyConstructor, int count)
    {
        Construct = enemyConstructor;
        Count = count;
    }
}