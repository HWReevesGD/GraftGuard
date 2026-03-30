using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies.Animation
{
    public class AnimationClip
    {
        public string ClipName { get; set; } = "Clip Name";
        public float StrideLength { get; set; } = 0.1f;    // How fast the timer moves per pixel
        public float BobIntensity { get; set; } = 2.0f;    // Vertical bounce
        public float SwayIntensity { get; set; } = 3.0f;   // Side-to-side weight shift
        public float LimbWobbleScale { get; set; } = 0.2f; // How far limbs swing
        public float LeanFactor { get; set; } = 0.1f;      // How much the torso tilts into movement
        public bool IsStatic { get; set; } = false;        // If true, uses time instead of distance
    }
}
