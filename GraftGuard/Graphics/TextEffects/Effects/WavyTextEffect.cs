using Microsoft.Xna.Framework;
using System;

namespace GraftGuard.Graphics.TextEffects.Effects;

/// <summary>
/// Text effect that makes each letter go on a wave
/// </summary>
internal class WavyTextEffect : ITextEffect
{
    private float amplitude;
    private float frequency;

    /// <summary>
    /// Create text wave effect
    /// </summary>
    /// <param name="amplitude">Wave amplitude</param>
    /// <param name="frequency">Wave frequency (default: 1)</param>
    public WavyTextEffect(float amplitude, float frequency = 1)
    {
        this.amplitude = amplitude;
        this.frequency = frequency;
    }

    public void Update(GameTime gameTime) { }

    /// <summary>
    /// Apply wave effect to each letter
    /// </summary>
    /// <param name="index">Letter index</param>
    /// <param name="gameTime">GameTime</param>
    /// <param name="letter">Letter struct</param>
    /// <returns>Modified Letter struct</returns>
    public Letter DoEffect(int index, GameTime gameTime, Letter letter)
    {
        float yOffset = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * frequency + (float)index / 2) * amplitude;
        letter.Y += yOffset;
        return letter;
    }
}
