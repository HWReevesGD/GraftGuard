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
internal class Button
{
    public NinePatch Patch { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Rectangle Box => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    private MouseState _lastMouseState = new MouseState();
    private MouseState _thisMouseState = new MouseState();

    public bool IsMouseOver => Box.Contains(_thisMouseState.Position);
    public bool ClickedThisFrame => _lastMouseState.LeftButton == ButtonState.Released && _thisMouseState.LeftButton == ButtonState.Pressed && IsMouseOver;

    public Button(NinePatch patch, Vector2 position, Vector2 size)
    {
        Patch = patch;
        Position = position;
        Size = size;
    }

    /// <summary>
    /// Updates the current State of the button
    /// </summary>
    public void Update()
    {
        // Update Mouse States
        _lastMouseState = _thisMouseState;
        _thisMouseState = Mouse.GetState();
    }

    public void Draw(SpriteBatch batch, Color? color = null)
    {
        Patch.Draw(batch, Position, Size, color);
    }
}
