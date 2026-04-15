using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Waves;

internal class NightWaveSet
{
    public string Name { get; init; }
    public List<NightWave> Waves { get; set; }
    // The round this wave set can appear in
    public int Round { get; set; }
    public NightWaveSet(string name, List<NightWave> waves, int round)
    {
        Name = name;
        Waves = waves;
        Round = round;
    }
}