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
    public Texture2D MainTexture;
    public Texture2D PressedTexture;
    public Texture2D HoverTexture;
    public NinePatch Patch { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Rectangle Box => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    private MouseState _lastMouseState = new MouseState();
    private MouseState _thisMouseState = new MouseState();

    public bool IsMouseOver => Box.Contains(_thisMouseState.Position);
    public bool IsPressed => IsMouseOver && _thisMouseState.LeftButton == ButtonState.Pressed;
    public bool ClickedThisFrame => _lastMouseState.LeftButton == ButtonState.Released && _thisMouseState.LeftButton == ButtonState.Pressed && IsMouseOver;

    /// <summary>
    /// Setup a button with the given <see cref="Texture2D"/>s and Margins. The Margins are used to construct a <see cref="NinePatch"/>
    /// <paramref name="pressedTexture"/> and <paramref name="hoverTexture"/> are optional textures used for the button in the appropriate states
    /// </summary>
    /// <param name="mainTexture">Main <see cref="Texture2D"/> of the button</param>
    /// <param name="leftMargin">Left pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="rightMargin">Right pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="topMargin">Top pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="bottomMargin">Bottom pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="pressedTexture">Pressed <see cref="Texture2D"/> of the button</param>
    /// <param name="hoverTexture">Hovered <see cref="Texture2D"/> of the button</param>
    public Button(Vector2 position, Vector2 size, Texture2D mainTexture, int leftMargin, int rightMargin, int topMargin, int bottomMargin, Texture2D? pressedTexture = null, Texture2D? hoverTexture = null, string text = "", SpriteFont font = null)
    {
        Position = position;
        Size = size;

        MainTexture = mainTexture;
        PressedTexture = pressedTexture ?? mainTexture;
        HoverTexture = hoverTexture ?? mainTexture;

        Patch = new NinePatch(mainTexture, leftMargin, rightMargin, topMargin, bottomMargin);
    }

    /// <summary>
    /// Updates the current State of the button
    /// </summary>
    public void Update()
    {
        // Update Mouse States
        _lastMouseState = _thisMouseState;
        _thisMouseState = Mouse.GetState();

        // Update Texture based on Button and Mouse States
        if (IsMouseOver && IsPressed)
        {
            Patch.Texture = PressedTexture;
        }
        else if (IsMouseOver)
        {
            Patch.Texture = HoverTexture;
        }
        else
        {
            Patch.Texture = MainTexture;
        }
    }

    public void Draw(SpriteBatch batch, Color? color = null)
    {
        Patch.Draw(batch, Position, Size, color);
    }
}
