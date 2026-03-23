using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies.Animation
{
    public class Animator
    {
        public float AnimationTimer { get; private set; }
        public AnimationClip CurrentClip { get; set; }
        public bool IsDebugMode { get; set; } = false;

        private Vector2 lastPosition;
        private float rotation;

        public Animator(AnimationClip startingClip)
        {
            CurrentClip = startingClip;
        }

        public void Update(GameTime gameTime, Vector2 currentPosition, float speedMultiplier = 1)
        {
            Vector2 velocity = (currentPosition - lastPosition) * speedMultiplier;
            float distance = velocity.Length() ;


            // Skip state switching if debugging a specific loop
            if (!IsDebugMode)
            {
                if (distance > 0.1f)
                    CurrentClip = AnimationClips.Walk;
                else
                    CurrentClip = AnimationClips.Idle;
            }
            else
            {
                distance = 1f;
            }

            // Advance Timer
            if (CurrentClip.IsStatic)
                AnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * CurrentClip.StrideLength;
            else
                AnimationTimer += distance * CurrentClip.StrideLength;

            // Calculate procedural rotation (leaning)
            float leanTarget = (distance > 0) ? velocity.X * CurrentClip.LeanFactor : 0;
            rotation = MathHelper.Lerp(rotation, leanTarget, 0.1f);

            lastPosition = currentPosition;
        }

        public float GetRotation() => rotation;
    }
}
