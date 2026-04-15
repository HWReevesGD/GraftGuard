using System.Collections.Generic;

namespace GraftGuard.Map.Waves;

internal class NightWave
{
    public List<SpawnConfig> Spawns { get; set; }
    public float WaveDelay = 0.0f;
    public float NextWaveDelay = 0.0f;
    public NightWave(List<SpawnConfig> spawns, float delayBeforeStarting = 0.0f, float delayUntilNext = 0.0f)
    {
        Spawns = spawns;
        WaveDelay = delayBeforeStarting;
        NextWaveDelay = delayUntilNext;
    }
}