// builder-style text params

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GraftGuard.Graphics.TextEffects;

/// <summary>
/// X origin position
/// </summary>
public enum XOrigin
{
    Left,
    Center,
    Right
}

/// <summary>
/// Y origin position
/// </summary>
public enum YOrigin
{
    Top,
    Center,
    Bottom
}

/// <summary>
/// Text parameters struct. Easily usable via method chaining.
/// </summary>
internal struct Text
{
    public SpriteFont Font;
    public string TextString;
    private List<ITextEffect> effects;

    /// <summary>
    /// Gets the size of the text without kerning
    /// </summary>
    private Vector2 BaseSize => Font.MeasureString(TextString) * Scale;

    /// <summary>
    /// Gets the size of the text with kerning included
    /// </summary>
    public Vector2 Size => BaseSize + new Vector2((TextString.Length - 1) * Kerning * Scale.X, 0);

    /// <summary>
    /// Gets the width of the text with kerning included
    /// </summary>
    public int Width => (int)Size.X;
    /// <summary>
    /// Gets the height of the text with kerning included
    /// </summary>
    public int Height => (int)Size.Y;

    public int Kerning = 0;
    public XOrigin XOrigin = XOrigin.Left;
    public YOrigin YOrigin = YOrigin.Top;
    public Color Color = Color.White;
    public Vector2 Scale = new Vector2(1, 1);

    /// <summary>
    /// Gets the draw origin of this text
    /// </summary>
    public Vector2 Origin
    {
        get
        {
            float x = XOrigin switch
            {
                XOrigin.Left => 0,
                XOrigin.Center => -Size.X / 2,
                XOrigin.Right => -Size.X
            };

            float y = YOrigin switch
            {
                YOrigin.Top => 0,
                YOrigin.Center => -Size.Y / 2,
                YOrigin.Bottom => -Size.Y
            };

            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// Create text parameters with this font and text
    /// </summary>
    /// <param name="font">SpriteFont</param>
    /// <param name="text">string</param>
    public Text(SpriteFont font, string text)
    {
        Font = font;
        TextString = text;
        effects = new List<ITextEffect>();
    }

    /// <summary>
    /// Set the kerning of the text (space between letters)
    /// </summary>
    /// <param name="newKerning">Kerning</param>
    /// <returns>this</returns>
    public Text SetKerning(int newKerning)
    {
        Kerning = newKerning;
        return this;
    }

    /// <summary>
    /// Set the color of the text
    /// </summary>
    /// <param name="color">Text color</param>
    /// <returns>this</returns>
    public Text SetColor(Color color)
    {
        Color = color;
        return this;
    }

    /// <summary>
    /// Set X origin position
    /// </summary>
    /// <param name="xOrigin">X origin</param>
    /// <returns>this</returns>
    public Text SetXOrigin(XOrigin xOrigin)
    {
        XOrigin = xOrigin;
        return this;
    }

    /// <summary>
    /// Set Y origin position
    /// </summary>
    /// <param name="yOrigin">Y origin</param>
    /// <returns>this</returns>
    public Text SetYOrigin(YOrigin yOrigin)
    {
        YOrigin = yOrigin;
        return this;
    }

    /// <summary>
    /// Set both X and Y origin
    /// </summary>
    /// <param name="xOrigin">X origin</param>
    /// <param name="yOrigin">Y orogin</param>
    /// <returns>this</returns>
    public Text SetOrigin(XOrigin xOrigin, YOrigin yOrigin)
    {
        SetXOrigin(xOrigin);
        SetYOrigin(yOrigin);
        return this;
    }

    /// <summary>
    /// Set scale vector
    /// </summary>
    /// <param name="scale">Scale vector</param>
    /// <returns>this</returns>
    public Text SetScale(Vector2 scale)
    {
        Scale = scale;
        return this;
    }

    /// <summary>
    /// Set scale on both axes
    /// </summary>
    /// <param name="scale">Scale</param>
    /// <returns>this</returns>
    public Text SetScale(float scale)
    {
        Scale = new Vector2(scale, scale);
        return this;
    }

    /// <summary>
    /// Add text effect to this handler
    /// </summary>
    /// <param name="effect">Effect to add</param>
    /// <returns>This handler</returns>
    public Text AddEffect(ITextEffect effect)
    {
        effects.Add(effect);
        return this;
    }

    /// <summary>
    /// Add multiple text effects to this handler
    /// </summary>
    /// <param name="effects">Array of effects</param>
    /// <returns>This handler</returns>
    public Text AddEffects(ITextEffect[] effects)
    {
        foreach (ITextEffect effect in effects)
        {
            this.effects.Add(effect);
        }
        return this;
    }

    /// <summary>
    /// Draw each letter separately to add kerning without text effects
    /// </summary>
    /// <param name="drawing">SpriteBatch</param>
    /// <param name="position">Draw position</param>
    private void DrawRawWithKerning(DrawManager drawing, Vector2 position, bool isUi, int drawLayer = 2)
    {
        Vector2 leftPosition = position + Origin;

        foreach (char character in TextString)
        {
            string charString = $"{character}";

            drawing.DrawString(
                font: Font,
                text: charString,
                position: leftPosition,
                color: Color,
                scale: Scale,
                isUi: isUi,
                drawLayer: drawLayer
                );

            // advanced by width and kerning
            leftPosition += new Vector2((Font.MeasureString(charString).X + Kerning) * Scale.X, 0);
        }
    }

    /// <summary>
    /// Draw this text without text effects
    /// </summary>
    /// <param name="drawing">SpriteBatch</param>
    /// <param name="position">Draw position</param>
    public void DrawRaw(DrawManager drawing, Vector2 position, bool isUi = true, int drawLayer = 2)
    {
        if (Kerning != 0)
        {
            DrawRawWithKerning(drawing, position, isUi);
            return;
        }

        drawing.DrawString(
            font: Font,
            text: TextString,
            position: position + Origin,
            color: Color,
            scale: Scale,
            isUi: isUi,
            drawLayer: drawLayer
            );
    }

    /// <summary>
    /// Draw the text provided at construct time with the new effects added
    /// </summary>
    /// <param name="drawing">SpriteBatch</param>
    /// <param name="gameTime">GameTime</param>
    /// <param name="position">Text origin position on screen</param>
    public void Draw(DrawManager drawing, GameTime gameTime, Vector2 position, bool isUi = true, int drawLayer = 2)
    {
        Vector2 renderOrigin = position + Origin;
        Vector2 curPosition = renderOrigin;

        // update all effects
        foreach (ITextEffect effect in effects)
        {
            effect.Update(gameTime);
        }

        for (int i = 0; i < TextString.Length; i++)
        {
            string character = $"{TextString[i]}";

            // apply all effects to individual characters
            Letter letter = new Letter(character);
            letter.Position = curPosition;
            letter.Scale = Scale;
            letter.Color = Color;

            foreach (ITextEffect effect in effects)
            {
                letter = effect.DoEffect(i, gameTime, letter);
            }

            // draw this letter
            drawing.DrawString(
                text: character,
                position: letter.Position,
                font: Font,
                color: letter.Color,
                scale: letter.Scale,
                isUi: isUi,
                drawLayer: drawLayer
                );

            // advance position
            curPosition += new Vector2((Font.MeasureString(character).X + Kerning) * Scale.X, 0);
        }
    }
}
