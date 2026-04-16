using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Waves;
internal class WaveManager
{
    public NightWaveSet Set { get; private set; }
    public float FullTime => Set.FullTime;
    public Dictionary<NightWave, bool> Started { get; private set; } = [];
    public Action<NightWave> SpawnWave;
    public void StartWaves(NightWaveSet waveSet)
    {
        Started.Clear();
        Set = waveSet;
        foreach (NightWave wave in waveSet.Waves)
        {
            Started[wave] = false;
        }
        NightWave first = waveSet.Waves[0];
        Started[first] = true;
        SpawnWave?.Invoke(first);
    }
    public void Update(float timeLeft)
    {
        foreach ((NightWave wave, bool started) in Started)
        {
            if (started)
            {
                continue;
            }
            float startTime = Set.StartTimes[wave];
            if (FullTime - timeLeft > startTime)
            {
                Started[wave] = true;
                SpawnWave?.Invoke(wave);
            }
        }
    }
}
