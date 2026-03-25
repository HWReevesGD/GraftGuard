using Microsoft.Xna.Framework;
using System;

namespace GraftGuard.Graphics.TextEffects;

/// <summary>
/// Text effect that shakes each letter
/// </summary>
internal class ShakeTextEffect : ITextEffect
{
    private float magnitude;
    private Random rng;

    private bool doShakeThisFrame;

    /// <summary>
    /// Create text shaking effect
    /// </summary>
    /// <param name="magnitude">Shake magnitude</param>
    public ShakeTextEffect(float magnitude)
    {
        this.magnitude = magnitude;
        rng = new Random();
    }

    public void Update(GameTime gameTime) { }

    /// <summary>
    /// Shake each letter
    /// </summary>
    /// <param name="index">Letter index</param>
    /// <param name="gameTime">GameTime</param>
    /// <param name="letter">Letter struct</param>
    /// <returns>Modified Letter struct</returns>
    public Letter DoEffect(int index, GameTime gameTime, Letter letter)
    {
        float xMul = -1 + (float)rng.NextDouble() * 2;
        float yMul = -1 + (float)rng.NextDouble() * 2;
        letter.Position += new Vector2(xMul, yMul) * magnitude;
        return letter;
    }
}
