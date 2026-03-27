using Microsoft.Xna.Framework;

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
