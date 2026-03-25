// builder-style text params

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
public struct Text
{
    public SpriteFont Font;

    /// <summary>
    /// Gets the size of the text without kerning
    /// </summary>
    private Vector2 BaseSize => Font.MeasureString(TextString) * Scale;

    /// <summary>
    /// Gets the size of the text with kerning included
    /// </summary>
    public Vector2 Size => BaseSize + new Vector2((TextString.Length - 1) * Kerning * Scale.X, 0);

    public string TextString;

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
            float x = 0;
            float y = 0;

            switch (XOrigin)
            {
                case XOrigin.Left:
                    x = 0;
                    break;
                case XOrigin.Center:
                    x = -Size.X / 2;
                    break;
                case XOrigin.Right:
                    x = -Size.X;
                    break;
            }

            switch (YOrigin)
            {
                case YOrigin.Top:
                    y = 0;
                    break;
                case YOrigin.Center:
                    y = -Size.Y / 2;
                    break;
                case YOrigin.Bottom:
                    y = -Size.Y;
                    break;
            }

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
    /// Draw each letter separately to add kerning
    /// </summary>
    /// <param name="batch">SpriteBatch</param>
    /// <param name="position">Draw position</param>
    private void DrawWithKerning(SpriteBatch batch, Vector2 position)
    {
        Vector2 leftPosition = position + Origin;

        foreach (char character in TextString)
        {
            string charString = $"{character}";

            batch.DrawString(
                Font,
                charString,
                leftPosition,
                Color,
                0,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0
                );

            // advanced by width and kerning
            leftPosition += new Vector2((Font.MeasureString(charString).X + Kerning) * Scale.X, 0);
        }
    }

    /// <summary>
    /// Draw this text without text effects
    /// </summary>
    /// <param name="batch">SpriteBatch</param>
    /// <param name="position">Draw position</param>
    public void Draw(SpriteBatch batch, Vector2 position)
    {
        if (Kerning != 0)
        {
            DrawWithKerning(batch, position);
            return;
        }

        batch.DrawString(
            Font,
            TextString,
            position + Origin,
            Color,
            0,
            Vector2.Zero,
            Scale,
            SpriteEffects.None,
            0
            );
    }
}
