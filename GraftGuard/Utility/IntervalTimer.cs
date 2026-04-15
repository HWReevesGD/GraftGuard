using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Utility;
internal class IntervalTimer
{
    public float Interval { get; init; }
    private float _timeSinceLastInterval;

    public IntervalTimer(float interval, float initialTimeOffset = 0.0f)
    {
        Interval = interval;
        _timeSinceLastInterval = 0.0f + initialTimeOffset;
    }

    public virtual bool Update(GameTime time)
    {
        _timeSinceLastInterval += (float)time.ElapsedGameTime.TotalSeconds;

        if(_timeSinceLastInterval >= Interval)
        {
            _timeSinceLastInterval -= Interval;
            return true;
        }
        return false;
    }

    public virtual void Reset()
    {
        _timeSinceLastInterval = 0.0f;
    }
}
