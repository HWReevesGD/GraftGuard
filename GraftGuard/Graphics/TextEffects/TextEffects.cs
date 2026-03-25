using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GraftGuard.Graphics.TextEffects;

/// <summary>
/// Letter struct to allow each letter to be individually manipulated
/// </summary>
public struct Letter
{
    public string LetterChar;
    public Vector2 Position;

    /// <summary>
    /// Gets and sets the X position
    /// </summary>
    public float X { get => Position.X; set => Position = new Vector2(value, Position.Y); }
    /// <summary>
    /// Gets and sets the Y position
    /// </summary>
    public float Y { get => Position.Y; set => Position = new Vector2(Position.X, value); }

    public Vector2 Scale;
    public Color Color;

    /// <summary>
    /// Create letter struct
    /// </summary>
    /// <param name="letter">Letter (string)</param>
    public Letter(string letter)
    {
        LetterChar = letter;
    }

    /// <summary>
    /// Create letter struct
    /// </summary>
    /// <param name="letter">Letter (char)</param>
    public Letter(char letter)
    {
        LetterChar = $"{letter}";
    }
}

/// <summary>
/// Text effect handler, can handle multiple effects. Easily usable via method chaining.
/// </summary>
internal class TextEffects
{
    private Text textParams;
    private List<ITextEffect> effects;

    /// <summary>
    /// Create text effect handler with the given text params
    /// </summary>
    /// <param name="textParams">Text</param>
    public TextEffects(Text textParams)
    {
        this.textParams = textParams;
        effects = new List<ITextEffect>();
    }

    /// <summary>
    /// Create text effect handler with the default text params
    /// </summary>
    /// <param name="font">SpriteFont</param>
    /// <param name="text">string</param>
    public TextEffects(SpriteFont font, string text) : this(new Text(font, text)) { }

    /// <summary>
    /// Add text effect to this handler
    /// </summary>
    /// <param name="effect">Effect to add</param>
    /// <returns>This handler</returns>
    public TextEffects AddEffect(ITextEffect effect)
    {
        effects.Add(effect);
        return this;
    }

    /// <summary>
    /// Add multiple text effects to this handler
    /// </summary>
    /// <param name="effects">Array of effects</param>
    /// <returns>This handler</returns>
    public TextEffects AddEffects(ITextEffect[] effects)
    {
        foreach (ITextEffect effect in effects)
        {
            this.effects.Add(effect);
        }
        return this;
    }

    /// <summary>
    /// Draw the text provided at construct time with the new effects added
    /// </summary>
    /// <param name="batch">SpriteBatch</param>
    /// <param name="gameTime">GameTime</param>
    /// <param name="position">Text origin position on screen</param>
    public void Draw(SpriteBatch batch, GameTime gameTime, Vector2 position)
    {
        Vector2 renderOrigin = position + textParams.Origin;
        Vector2 curPosition = renderOrigin;

        // update all effects
        foreach (ITextEffect effect in effects)
        {
            effect.Update(gameTime);
        }

        for (int i = 0; i < textParams.TextString.Length; i++)
        {
            string character = $"{textParams.TextString[i]}";

            // apply all effects to individual characters
            Letter letter = new Letter(character);
            letter.Position = curPosition;
            letter.Scale = textParams.Scale;
            letter.Color = textParams.Color;

            foreach (ITextEffect effect in effects)
            {
                letter = effect.DoEffect(i, gameTime, letter);
            }

            // draw this letter
            batch.DrawString(
                textParams.Font,
                character,
                letter.Position,
                letter.Color,
                0,
                Vector2.Zero,
                letter.Scale,
                SpriteEffects.None,
                0
                );

            // advance position
            curPosition += new Vector2(
                (textParams.Font.MeasureString(character).X + textParams.Kerning) * textParams.Scale.X,
                0
                );
        }
    }
}
