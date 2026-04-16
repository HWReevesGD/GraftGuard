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
        Func<Vector2, EnemyManager, Enemy> humanoid = (position, _) => new EnemyHumanoid(position);
        Func<Vector2, EnemyManager, Enemy> centipede = (position, manager) => new EnemyCentipede(position, manager);

        WaveSet("Humanoid Easy",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ], 30.0f)
                ], round: 0);

        WaveSet("Centipede Introduction",
                [
            new NightWave([
                        new SpawnConfig(humanoid, 2),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(humanoid, 1),
                        new SpawnConfig(centipede, 1),
                    ], 30.0f)
                ], round: 1);

        WaveSet("Humanoid Dual",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ], 25.0f)
                ], round: 3);

        WaveSet("Centipede Dual",
                [
                    new NightWave([
                        new SpawnConfig(centipede, 1),
                        new SpawnConfig(humanoid, 1),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(centipede, 1),
                        new SpawnConfig(humanoid, 1),
                    ], 25.0f)
                ], round: 3);
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
