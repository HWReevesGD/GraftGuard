// builder-style text params

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GraftGuard.Graphics.TextEffects;

public enum HorizontalAlignment
{
    Left,
    Center,
    Right
}

public enum VerticalAlignment
{
    Top,
    Center,
    Bottom
}

public struct TextParams
{
    public SpriteFont Font;
    private Vector2 baseSize => Font.MeasureString(Text);
    public Vector2 Size => baseSize + new Vector2((Text.Length - 1) * Kerning, 0);

    public string Text;
    public int Width => (int)Size.X;
    public int Height => (int)Size.Y;
    public int Kerning;
    public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left;
    public VerticalAlignment VerticalAlignment = VerticalAlignment.Top;
    public Color Color;
    public Vector2 Scale;
    public float Rotation;

    public Vector2 Origin
    {
        get
        {
            float x = 0;
            float y = 0;

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    x = 0;
                    break;
                case HorizontalAlignment.Center:
                    x = Size.X / 2;
                    break;
                case HorizontalAlignment.Right:
                    x = Size.X;
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    y = 0;
                    break;
                case VerticalAlignment.Center:
                    y = Size.Y / 2;
                    break;
                case VerticalAlignment.Bottom:
                    y = Size.Y;
                    break;
            }

            return new Vector2(x, y);
        }
    }

    public TextParams(SpriteFont font, string text)
    {
        Font = font;
        Text = text;
    }

    public TextParams SetKerning(int newKerning)
    {
        Kerning = newKerning;
        return this;
    }

    public TextParams SetColor(Color color)
    {
        Color = color;
        return this;
    }

    public TextParams SetHorizontalAlignment(HorizontalAlignment alignment)
    {
        HorizontalAlignment = alignment;
        return this;
    }

    public TextParams SetVerticalAlignment(VerticalAlignment alignment)
    {
        VerticalAlignment = alignment;
        return this;
    }

    public TextParams SetAlignment(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
    {
        SetHorizontalAlignment(horizontalAlignment);
        SetVerticalAlignment(verticalAlignment);
        return this;
    }

    public TextParams SetScale(Vector2 scale)
    {
        Scale = scale;
        return this;
    }

    public TextParams SetScale(float scale)
    {
        Scale = new Vector2(scale, scale);
        return this;
    }

    public TextParams SetRotation(float rotation)
    {
        Rotation = rotation;
        return this;
    }
}
