using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GraftGuard.Graphics.Particles;

/// <summary>
/// Particle manager. Holds, updates, and draws particles in bulk.
/// </summary>
internal class ParticleManager
{
    private List<Particle> particles = new List<Particle>();

    /// <summary>
    /// Add particle to this ParticleManager. Triggers Particle to finalize fields
    /// </summary>
    /// <param name="particle">Particle</param>
    /// <returns>this</returns>
    public ParticleManager Add(Particle particle)
    {
        // finalize the particle
        particle.SetInitialVelocity();

        particles.Add(particle);
        return this;
    }

    /// <summary>
    /// Instantly clear all particles
    /// </summary>
    public void Clear()
    {
        particles.Clear();
    }

    /// <summary>
    /// Step particles forward
    /// </summary>
    /// <param name="gameTime">This frame's GameTime</param>
    public void Update(GameTime gameTime)
    {
        List<Particle> toRemove = new List<Particle>();

        foreach (Particle particle in particles)
        {
            bool isAlive = particle.Update(gameTime);
            if (!isAlive)
                toRemove.Add(particle);
        }

        foreach (Particle removed in toRemove)
        {
            particles.Remove(removed);
        }
    }

    /// <summary>
    /// Draw particles to the screen
    /// </summary>
    /// <param name="drawing">SpriteBatch</param>
    /// <param name="gameTime">This frame's GameTime</param>
    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        foreach (Particle particle in particles)
        {
            particle.Draw(drawing, gameTime);
        }
    }
}
