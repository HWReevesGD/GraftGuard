using System;
using System.Collections.Generic;

namespace GraftGuard.Map.Waves;

internal class NightWave : IEquatable<NightWave>
{
    private static int _nextId = int.MinValue;
    private int _id;
    public List<SpawnConfig> Spawns { get; init; }
    public float Time { get; init; }
    public NightWave(List<SpawnConfig> spawns, float time)
    {
        Spawns = spawns;
        Time = time;
        _id = _nextId++;
    }
    public override int GetHashCode()
    {
        return _id;
    }
    public override bool Equals(object obj)
    {
        return obj is NightWave wave && wave._id == _id;
    }
    public bool Equals(NightWave other)
    {
        return other._id == _id;
    }
}