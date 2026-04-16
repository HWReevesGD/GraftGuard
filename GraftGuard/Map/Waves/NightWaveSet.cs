using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Waves;

internal class NightWaveSet
{
    public string Name { get; init; }
    public List<NightWave> Waves { get; init; }
    // The round this wave set can appear in
    public int Round { get; init; }
    public float FullTime { get; init; }
    public FrozenDictionary<NightWave, float> StartTimes;
    public NightWaveSet(string name, List<NightWave> waves, int round)
    {
        Name = name;
        Waves = waves;
        Round = round;
        FullTime = waves.Sum((wave) => wave.Time);
        Dictionary<NightWave, float> completionTimes = [];
        float currentTime = 0.0f;
        foreach (NightWave wave in waves)
        {
            completionTimes[wave] = currentTime;
            currentTime += wave.Time;
        }
        StartTimes = completionTimes.ToFrozenDictionary();
    }
}