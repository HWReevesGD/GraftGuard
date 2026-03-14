using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;
internal class PatchButton : Button
{
    public Texture2D MainTexture;
    public Texture2D PressedTexture;
    public Texture2D HoverTexture;
    public NinePatch Patch { get; set; }
    public bool FitIconToPatchMargins { get; set; }

    private MouseState _lastMouseState = new MouseState();
    private MouseState _thisMouseState = new MouseState();

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
        ButtonIconType iconType = ButtonIconType.Fixed, bool fitIconToPatchMargins = true) : base(position, size, text, textColor, font, icon, iconType)
    {

        MainTexture = mainTexture;
        PressedTexture = pressedTexture ?? mainTexture;
        HoverTexture = hoverTexture ?? mainTexture;

        Patch = new NinePatch(mainTexture, leftMargin, rightMargin, topMargin, bottomMargin);
        FitIconToPatchMargins = fitIconToPatchMargins;
    }

    /// <summary>
    /// Updates the current State of the button
    /// </summary>
    public override void Update()
    {
        base.Update();

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

    public override void Draw(SpriteBatch batch, Color? color = null)
    {
        Patch.Draw(batch, Position, Size, color);
        base.Draw(batch, color);
    }

    protected override void DrawIcon(SpriteBatch batch)
    {
        if(!FitIconToPatchMargins)
        {
            base.DrawIcon(batch);
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
        batch.Draw(Icon, destinationRectangle, Color.White);
    }
}
