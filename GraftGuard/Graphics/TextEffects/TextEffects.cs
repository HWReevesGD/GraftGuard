using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Graphics.TextEffects;

public struct Letter
{
    public string LetterChar;
    public Vector2 Position;
    public float X { get => Position.X; set => Position = new Vector2(value, Position.Y); }
    public float Y { get => Position.Y; set => Position = new Vector2(Position.X, value); }
    public float Rotation;
    public Vector2 Scale;

    public Vector2 Up;
    public Vector2 Right;
    public Color Color;

    public Letter(string letter)
    {
        LetterChar = letter;
    }

    public Letter(char letter)
    {
        LetterChar = $"{letter}";
    }
}

internal class TextEffects
{
    public static void testplsdontlookat()
    {
        TextParams param = new TextParams(Fonts.Arial, "Hello!")
            .SetHorizontalAlignment(HorizontalAlignment.Right)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetKerning(10)
            .SetScale(2)
            .SetColor(Color.Red)
            .SetRotation(45);

    }

    private TextParams textParams;
    private List<ITextEffect> effects;

    //public static TextEffect Wavy(float amplitude, float frequency, TextParams textParams)
    //{
    //    return new TextEffect()
    //}

    public TextEffects(TextParams textParams)
    {
        this.textParams = textParams;
        effects = new List<ITextEffect>();
    }

    public TextEffects AddEffect(ITextEffect effect)
    {
        effects.Add(effect);
        return this;
    }

    public void Draw(SpriteBatch batch, GameTime gameTime, Vector2 position)
    {
        float rotation = textParams.Rotation;
        Vector2 up = new Vector2((float)Math.Sin(rotation), (float)Math.Cos(rotation));
        Vector2 right = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

        Vector2 renderOrigin = position - (right * textParams.Width / 2) - (up * textParams.Height / 2);
        Vector2 curPosition = renderOrigin;

        foreach (char character in textParams.Text)
        {
            Letter letter = new Letter(character);
            letter.Up = up;
            letter.Right = right;
            letter.Position = curPosition;
            letter.Rotation = textParams.Rotation;
            letter.Scale = textParams.Scale;
            letter.Color = textParams.Color;

            foreach (ITextEffect effect in effects)
            {
                letter = effect.DoEffect(gameTime, letter);
            }

            batch.DrawString(
                textParams.Font,
                textParams.Text,
                letter.Position,
                letter.Color,
                letter.Rotation,
                new Vector2(),
                letter.Scale,
                SpriteEffects.None,
                0
                );

            curPosition += right * (textParams.Font.MeasureString($"{character}").X + textParams.Kerning);
        }
    }
}
