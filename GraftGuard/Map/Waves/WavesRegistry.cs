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
        Func<Vector2, EnemyManager, Enemy> arachnid = (position, manager) => new EnemyArachnid(position);

        WaveSet("Humanoid Easy",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ], 50.0f)
                ], round: 0);

        WaveSet("Centipede Introduction",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 2),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(humanoid, 1),
                        new SpawnConfig(centipede, 1),
                    ], 40.0f)
                ], round: 1);

        WaveSet("Humanoid Dual",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(humanoid, 3),
                    ], 40.0f)
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
                    ], 40.0f)
                ], round: 3);

        WaveSet("Centipede Bombardment",
                [
                    new NightWave([
                        new SpawnConfig(centipede, 4),
                        new SpawnConfig(humanoid, 1),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(centipede, 3),
                        new SpawnConfig(humanoid, 2),
                    ], 40.0f)
                ], round: 4);

        WaveSet("Humanoid Medium",
                [
                    new NightWave([
                        new SpawnConfig(humanoid, 6),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(centipede, 1),
                        new SpawnConfig(humanoid, 5),
                    ], 40.0f)
                ], round: 4);

        WaveSet("Arachnid Intro",
                [
                    new NightWave([
                        new SpawnConfig(arachnid, 1),
                        new SpawnConfig(humanoid, 3),
                    ], 20.0f),
                    new NightWave([
                        new SpawnConfig(centipede, 4),
                        new SpawnConfig(humanoid, 1),
                    ], 30.0f)
                ], round: 4);

        WaveSet("Infestation",
                [
                    new NightWave([
                        new SpawnConfig(centipede, 2),
                    ], 5.0f),
                    new NightWave([
                        new SpawnConfig(centipede, 2),
                    ], 5.0f),
                    new NightWave([
                        new SpawnConfig(centipede, 2),
                    ], 5.0f),
                    new NightWave([
                        new SpawnConfig(centipede, 2),
                    ], 5.0f),
                    new NightWave([
                        new SpawnConfig(arachnid, 2),
                    ], 30.0f),
                ], round: 5);

        WaveSet("Dual Walkers",
                [
                    new NightWave([
                        new SpawnConfig(arachnid, 1),
                    ], 5.0f),
                    new NightWave([
                        new SpawnConfig(humanoid, 2),
                    ], 5.0f),
                    new NightWave([
                        new SpawnConfig(humanoid, 2),
                    ], 10.0f),
                    new NightWave([
                        new SpawnConfig(arachnid, 1),
                    ], 30.0f),
                ], round: 5);
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
