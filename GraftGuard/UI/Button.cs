using GraftGuard.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;
internal abstract class Button
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Rectangle Box => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
    public string Text { get; set; }
    public SpriteFont Font { get; set; }
    public Color TextColor { get; set; }

    private MouseState _lastMouseState = new MouseState();
    private MouseState _thisMouseState = new MouseState();

    public bool IsMouseOver => Box.Contains(_thisMouseState.Position);
    public bool IsPressed => IsMouseOver && _thisMouseState.LeftButton == ButtonState.Pressed;
    public bool ClickedThisFrame => _lastMouseState.LeftButton == ButtonState.Released && _thisMouseState.LeftButton == ButtonState.Pressed && IsMouseOver;

    /// <summary>
    /// Setup a button with the given position and size
    /// </summary>
    /// <param name="position">Initial position of the <see cref="Button"/></param>
    /// <param name="size">Initial size of the <see cref="Button"/></param>
    public Button(Vector2 position, Vector2 size, string text = "", Color? textColor = null, SpriteFont font = null)
    {
        Position = position;
        Size = size;

        Text = text;

        // Use white as a default if a color is not given
        TextColor = textColor ?? Color.White;
        // Use Arial as a default font if a font is not given
        Font = font ?? Fonts.Arial;
    }

    /// <summary>
    /// Updates the current State of the button
    /// </summary>
    public virtual void Update()
    {
        // Update Mouse States
        _lastMouseState = _thisMouseState;
        _thisMouseState = Mouse.GetState();
    }

    public virtual void Draw(SpriteBatch batch, Color? color = null)
    {
        batch.DrawString(
            Font,
            Text,
            Position + Size / 2.0f - Font.MeasureString(Text) / 2.0f,
            TextColor
            );
    }
}
