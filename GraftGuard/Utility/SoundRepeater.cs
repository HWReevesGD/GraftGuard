using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Utility;
internal class SoundRepeater : IntervalTimer
{
    public SoundEffect Sound { get; set; }
    public SoundRepeater(SoundEffect sound, float interval, float initialTimeOffset = 0)
        : base(interval, initialTimeOffset)
    {
        Sound = sound;
    }
    public override bool Update(GameTime time)
    {
        bool play = base.Update(time);
        if (play)
        {
            Sound.Play();
        }
        return play;
    }
}
