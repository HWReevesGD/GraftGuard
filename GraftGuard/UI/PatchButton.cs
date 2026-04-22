using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard.UI;

internal delegate void Clicked();

internal class PatchButton : Button
{
    public Texture2D MainTexture;
    public Texture2D PressedTexture;
    public Texture2D HoverTexture;
    public NinePatch Patch { get; set; }
    public Rectangle MarginBox => Box.Translated(Patch.MarginTopLeft).LinearScaled(Patch.MarginAll.Negative());
    public bool FitIconToPatchMargins { get; set; }
    public bool FitTextToPatchMargins { get; set; }

    private MouseState _lastMouseState = new MouseState();
    private MouseState _thisMouseState = new MouseState();

    public event Clicked Clicked;

    /// <summary>
    /// Setup a button with the given <see cref="Texture2D"/>s and Margins. The Margins are used to construct a <see cref="NinePatch"/>
    /// <paramref name="pressedTexture"/> and <paramref name="hoverTexture"/> are optional textures used for the button in the appropriate states
    /// </summary>
    /// <param name="position">Initial position of the <see cref="Button"/></param>
    /// <param name="size">Initial size of the <see cref="Button"/></param>
    /// <param name="mainTexture">Main <see cref="Texture2D"/> of the button</param>
    /// <param name="leftMargin">Left pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="rightMargin">Right pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="topMargin">Top pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="bottomMargin">Bottom pixels of the <see cref="NinePatch"/> margin</param>
    /// <param name="pressedTexture">Pressed <see cref="Texture2D"/> of the button</param>
    /// <param name="hoverTexture">Hovered <see cref="Texture2D"/> of the button</param>
    public PatchButton(Vector2 position, Vector2 size, Texture2D mainTexture, int leftMargin, int rightMargin,
        int topMargin, int bottomMargin, Texture2D? pressedTexture = null, Texture2D? hoverTexture = null,
        string text = "", Color? textColor = null, SpriteFont font = null, Texture2D? icon = null,
        ButtonIconType iconType = ButtonIconType.Fixed, bool fitIconToPatchMargins = true, bool fitTextToPatchMargins = true) : base(position, size, text, textColor, font, icon, iconType)
    {

        MainTexture = mainTexture;
        PressedTexture = pressedTexture ?? mainTexture;
        HoverTexture = hoverTexture ?? mainTexture;

        Patch = new NinePatch(mainTexture, leftMargin, rightMargin, topMargin, bottomMargin);

        FitIconToPatchMargins = fitIconToPatchMargins;
        FitTextToPatchMargins = fitTextToPatchMargins;
    }

    public static PatchButton MakeBase(Vector2 position, Vector2 size, string text = "", Color? textColor = null, SpriteFont font = null, Texture2D? icon = null,
        ButtonIconType iconType = ButtonIconType.Fixed, bool fitIconToPatchMargins = true)
    {
        return new PatchButton(position, size, Placeholders.TextureButton, 6, 6, 11, 15, Placeholders.TextureButtonPressed, Placeholders.TextureButtonHover, text, textColor, font, icon, iconType, fitIconToPatchMargins);
    }
    public static PatchButton MakeBaseCentered(Vector2 position, Vector2 size, string text = "", Color? textColor = null, SpriteFont font = null, Texture2D? icon = null,
        ButtonIconType iconType = ButtonIconType.Fixed, bool fitIconToPatchMargins = true)
    {
        return MakeBase(position - size * 0.5f, size, text, textColor, font, icon, iconType, fitIconToPatchMargins);
    }

    /// <summary>
    /// Updates the current State of the button
    /// </summary>
    public override void Update(InputManager inputManager)
    {
        base.Update(inputManager);

        // Update Texture based on Button and Mouse States
        if (IsMouseHovered && IsPressed)
        {
            Patch.Texture = PressedTexture;
        }
        else if (IsMouseHovered)
        {
            Patch.Texture = HoverTexture;
        }
        else
        {
            Patch.Texture = MainTexture;
        }

        if (JustClicked)
        {
            Clicked?.Invoke();
        }
    }

    public override void Draw(DrawManager drawing, Color? color = null, Rectangle? scissor = null)
    {
        Patch.Draw(drawing, Position, Size.ToPoint(), color, sortMode: SortMode.Middle);
        base.Draw(drawing, color);
    }

    protected override void DrawIcon(DrawManager drawing)
    {
        if (Icon is null) return;
        if (!FitIconToPatchMargins)
        {
            base.DrawIcon(drawing);
            return;
        }

        Vector2 marginPosition = Position + Patch.MarginTopLeft.ToVector();
        Vector2 marginSize = Size - Patch.MarginAll.ToVector();

        Rectangle destinationRectangle = IconType switch
        {
            ButtonIconType.Fixed => new Rectangle((Position + Size * 0.5f - Icon.GetSize() * 0.5f).ToPoint(), Icon.GetSizePoint()),
            ButtonIconType.Stretch => new Rectangle(marginPosition.ToPoint(), marginSize.ToPoint()),
            ButtonIconType.AspectStretch => new Rectangle((marginPosition + marginSize * 0.5f - marginSize.SquareOfSmallest() * 0.5f).ToPoint(), marginSize.SquareOfSmallest().ToPoint())
        };
        drawing.Draw(Icon, destination: destinationRectangle, color: Color.White,
            isUi: true,
            sortMode: SortMode.Sorted);
    }

    protected override void DrawText(DrawManager drawing)
    {
        if (!FitTextToPatchMargins)
        {
            base.DrawText(drawing);
            return;
        }

        Vector2 marginOffset = Position + Patch.MarginTopLeft.ToVector();
        Vector2 marginSize = Size - Patch.MarginAll.ToVector();

        drawing.DrawString(
            font: Font,
            text: Text,
            position: marginOffset + marginSize / 2.0f - Font.MeasureString(Text) / 2.0f,
            color: TextColor,
            isUi: true,
            sortMode: SortMode.MiddleTop
            );
    }
}
