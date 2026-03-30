using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies.Animation
{
    public static class AnimationClips
    {

        public static AnimationClip Idle = new AnimationClip
        {
            ClipName = "Idle",
            IsStatic = true,
            StrideLength = 2.0f,
            BobIntensity = 1.0f,
            SwayIntensity = 0.5f,
            LimbWobbleScale = 0.05f,
            LeanFactor = 0
        };

        public static AnimationClip Walk = new AnimationClip
        {
            ClipName = "Walk",
            IsStatic = false,
            StrideLength = 0.1f,
            BobIntensity = 2.5f,
            SwayIntensity = 3.0f,
            LimbWobbleScale = 0.25f,
            LeanFactor = 0.15f
        };
    }
}
