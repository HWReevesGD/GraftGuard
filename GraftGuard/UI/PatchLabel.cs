using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.UI;
internal class PatchLabel : IMouseDetectable, IPositional, ISizeable
{
    public string Text { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Rectangle Box => new Rectangle(Position.ToPoint(), Size.ToPoint());
    public NinePatch Patch { get; set; }
    public SpriteFont Font { get; set; }
    public Color TextColor { get; set; }
    public PatchLabel(string text, Vector2 position, Vector2 size, Texture2D patchTexture, int marginLeft, int marginRight, int marginTop, int marginBottom, SpriteFont? font = null, Color? color = null)
    {
        Text = text;
        Position = position;
        Size = size;
        Patch = new NinePatch(patchTexture, marginLeft, marginRight, marginTop, marginBottom);
        // Optional defaults
        Font = font ?? Fonts.Arial;
        TextColor = color ?? Color.White;
    }
    public static PatchLabel MakeBase(string text, Vector2 position, Vector2 size)
    {
        return new PatchLabel(text, position, size, Placeholders.TexturePatchLabel, 5, 5, 5, 5);
    }
    public static PatchLabel MakeBaseCentered(string text, Vector2 position, Vector2 size)
    {
        return MakeBase(text, position - size * 0.5f, size);
    }
    public void Draw(DrawManager drawing, Color? color = null)
    {
        Patch.Draw(drawing, Position, size: Size.ToPoint(), color: color);
        drawing.DrawString(font: Font, text: Text, position: Position + Size * 0.5f - Font.MeasureString(Text) * 0.5f, color: TextColor);
    }

    public bool IsMouseOver(InputManager inputManager)
    {
        return new Rectangle(Position.ToPoint(), Size.ToPoint()).Contains(inputManager.MouseScreenPosition);
    }
}
