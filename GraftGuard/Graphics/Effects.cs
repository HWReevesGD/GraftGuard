
using GraftGuard.Graphics.Particles;
using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using System;

namespace GraftGuard.Graphics;

internal class Effects
{
    public static void DamageParticles(ParticleManager particles, Vector2 centerPosition)
    {
        Random rng = new Random();

        for (int i = 0; i < 35; i++)
        {
            Vector2 position = centerPosition + new Vector2(
                -20 + (float)rng.NextDouble() * 40,
                -20 + (float)rng.NextDouble() * 40
                );
            particles.Add(
                new Particle(Placeholders.TexturePixel)
                    .SetLifetime(0.5f, 1f)
                    .SetColor(Color.Red)
                    .SetSize(Vector2.One * 10, Vector2.Zero)
                    .SetSpeed(200f, 400f)
                    .SetAngle(-(float)Math.PI * 0.15f, -(float)Math.PI * 0.85f)
                    //.SetAngle(0, (float)Math.PI * 2)
                    .SetPosition(position)
                    .SetAcceleration(new Vector2(0, 2500))
            );
        }
    }
}
