using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard.UI;
enum ButtonIconType
{
    Fixed,
    Stretch,
    AspectStretch,
}
internal class Button : IMouseDetectable, IPositional, ISizeable
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Rectangle Box => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
    public string Text { get; set; }
    public SpriteFont Font { get; set; }
    public Color TextColor { get; set; }
    public Texture2D? Icon { get; set; }
    public ButtonIconType IconType { get; set; }
    public bool Disabled { get; set; } = false;
    public bool HideText { get; set; } = false;
    public Vector2 IconScale { get; set; } = Vector2.One;

    private MouseState _lastMouseState = new MouseState();
    private MouseState _thisMouseState = new MouseState();

    public bool IsMouseHovered => Box.Contains(_thisMouseState.Position);
    public bool IsPressed => IsMouseHovered && _thisMouseState.LeftButton == ButtonState.Pressed;
    public bool JustClicked => _lastMouseState.LeftButton == ButtonState.Released && _thisMouseState.LeftButton == ButtonState.Pressed && IsMouseHovered;

    

    /// <summary>
    /// Setup a button with the given position and size
    /// </summary>
    /// <param name="position">Initial position of the <see cref="Button"/></param>
    /// <param name="size">Initial size of the <see cref="Button"/></param>
    public Button(Vector2 position, Vector2 size, string text = "", Color? textColor = null, SpriteFont font = null, Texture2D? icon = null, ButtonIconType iconType = ButtonIconType.Fixed)
    {
        Position = position;
        Size = size;

        Text = text;

        // Use white as a default if a color is not given
        TextColor = textColor ?? Color.White;
        // Use Arial as a default font if a font is not given
        Font = font ?? Fonts.Arial;

        // Null Icons are fine, they will not be rendered
        Icon = icon;
        IconType = iconType;
    }

    /// <summary>
    /// Updates the current State of the button
    /// </summary>
    public virtual void Update(InputManager inputManager)
    {
        if (!Disabled)
        {
            // Update Mouse States
            _lastMouseState = _thisMouseState;
            _thisMouseState = inputManager.CurrentMouse;
        } else
        {
            _lastMouseState = new MouseState();
            _thisMouseState = new MouseState();
        }
    }

    public virtual void Draw(DrawManager drawing, Color? color = null, Rectangle? scissor = null)
    {
        if (Disabled)
        {
            drawing.Draw(Placeholders.TexturePixel, Box, sortMode: SortMode.Top, isUi: true, color: new Color(Color.Black, 0.7f));
        }
        DrawIcon(drawing);
        DrawText(drawing);
    }

    protected virtual void DrawText(DrawManager drawing)
    {
        if (HideText)
        {
            return;
        }
        drawing.DrawString(
            font: Font,
            text: Text,
            position: Position + Size / 2.0f - Font.MeasureString(Text) / 2.0f,
            color: TextColor,
            isUi: true
            );
    }
    protected virtual void DrawIcon(DrawManager drawing)
    {
        if (Icon is null) return;
        Rectangle destinationRectangle = IconType switch
        {
            ButtonIconType.Fixed => new Rectangle((Position + Size * 0.5f - Icon.GetSize() * IconScale * 0.5f).ToPoint(), Icon.GetSizePoint() * IconScale.ToPoint()),
            ButtonIconType.Stretch => new Rectangle(Position.ToPoint(), Size.ToPoint()),
            ButtonIconType.AspectStretch => new Rectangle((Position + Size * 0.5f - Size.SquareOfSmallest() * 0.5f).ToPoint(), Size.SquareOfSmallest().ToPoint())
        };
        drawing.Draw(Icon, destination: destinationRectangle, color: Color.White, isUi: true);
    }

    bool IMouseDetectable.IsMouseOver(InputManager inputManager)
    {
        return new Rectangle(Position.ToPoint(), Size.ToPoint()).Contains(inputManager.MouseScreenPosition);
    }
}
