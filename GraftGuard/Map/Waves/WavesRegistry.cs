using GraftGuard.Map.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GraftGuard.Map.Waves;

internal static class WavesRegistry
{
    public static Dictionary<int, List<NightWaveSet>> WaveSets = [];
    public static readonly Random Random = new Random();
    public static void LoadWaves()
    {
        Action<Vector2, EnemyManager> humanoid = (position, _) => new EnemyHumanoid(position);
        Action<Vector2, EnemyManager> centipede = (position, manager) => new EnemyCentipede(position, manager);

        WaveSet("Humanoid Only Easy",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ], 40.0f)
                ], round: 0);

        WaveSet("Centipede Introduction",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 1),
                        new SpawnConfig(centipede, 1),
                    ], 40.0f)
                ], round: 1);
    }

    public static void WaveSet(string name, List<NightWave> waves, int round)
    {
        List<NightWaveSet> sets = WaveSets.GetValueOrDefault(round, []);
        sets.Add(new NightWaveSet(name, waves, round));
        WaveSets[round] = sets;
    }

    public static List<NightWaveSet> GetRandomListForRound(int round)
    {
        int closestKey = 0;
        List<NightWaveSet> closestWaves = null;
        foreach ((int roundKey, List<NightWaveSet> waves) in WaveSets)
        {
            if (waves is null || waves.Count == 0)
            {
                continue;
            }

            if (closestWaves is null)
            {
                closestWaves = waves;
                closestKey = roundKey;
                continue;
            }

            if (Math.Abs(round - closestKey) > Math.Abs(round - roundKey))
            {
                closestKey = roundKey;
                closestWaves = waves;
            }
        }
        return closestWaves;
    }

    public static NightWaveSet GetRandomForRound(int round)
    {
        List<NightWaveSet> waveSets = GetRandomListForRound(round);
        return waveSets[Random.Next(waveSets.Count)];
    }
}
