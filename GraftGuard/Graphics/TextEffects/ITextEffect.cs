using Microsoft.Xna.Framework;

namespace GraftGuard.Graphics.TextEffects;

/// <summary>
/// Interface for text effects
/// </summary>
internal interface ITextEffect
{
    /// <summary>
    /// Update necessary items before doing effects on each letter
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void Update(GameTime gameTime);

    /// <summary>
    /// Called on for each letter to apply text effects
    /// </summary>
    /// <param name="index">Letter index</param>
    /// <param name="gameTime">GameTime</param>
    /// <param name="letter">Struct for one letter, modify this to apply text effects</param>
    /// <returns>Modified Letter struct</returns>
    public Letter DoEffect(int index, GameTime gameTime, Letter letter);
}
