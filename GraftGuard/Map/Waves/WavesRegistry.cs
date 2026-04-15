using GraftGuard.Map.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GraftGuard.Map.Waves;

internal static class WavesRegistry
{
    public static Dictionary<int, List<NightWaveSet>> WaveSets = [];
    public static void LoadWaves()
    {
        Action<Vector2, EnemyManager> humanoid = (position, _) => new EnemyHumanoid(position);
        Action<Vector2, EnemyManager> centipede = (position, manager) => new EnemyCentipede(position, manager);

        WaveSet("Humanoid Only Easy",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ])
                ], round: 0);

        WaveSet("Centipede Introduction",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 1),
                        new SpawnConfig(centipede, 1),
                    ])
                ], round: 1);
    }

    public static void WaveSet(string name, List<NightWave> waves, int round)
    {
        List<NightWaveSet> sets = WaveSets.GetValueOrDefault(round, []);
        sets.Add(new NightWaveSet(name, waves, round));
        WaveSets[round] = sets;
    }
}
