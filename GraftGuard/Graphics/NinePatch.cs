using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.Graphics;

/// <summary>
/// Helper class for rendering Nine-Patches. Mostly used for UI
/// </summary>
internal class NinePatch
{
    public Texture2D Texture { get; set; }
    public int MarginLeft { get; set; }
    public int MarginRight { get; set; }
    public int MarginTop { get; set; }
    public int MarginBottom { get; set; }
    public Point MarginTopLeft => new Point(MarginLeft, MarginTop);
    public Point MarginBottomRight => new Point(MarginRight, MarginBottom);
    public Point MarginAll => new Point(MarginLeft + MarginRight, MarginTop + MarginBottom);
    public NinePatch(Texture2D texture, int marginLeft, int marginRight, int marginTop, int marginBottom)
    {
        Texture = texture;
        MarginLeft = marginLeft;
        MarginRight = marginRight;
        MarginTop = marginTop;
        MarginBottom = marginBottom;
    }
    public void Draw(SpriteBatch batch, Vector2 position, Point size, Color? color = null)
    {
        // The Position of the Nine-Patch turned into a Point
        Point offset = position.ToPoint();
        // The Size of the Side's longest sides
        Point sideSize = size - new Point(MarginLeft + MarginRight, MarginTop + MarginBottom);
        // The Size of the Texture of the Nine-Patch
        Point textureSize = Texture.GetSizePoint();
        Point textureSideSize = textureSize - new Point(MarginLeft + MarginRight, MarginTop + MarginBottom);

        // Don't let the size be too small
        if (size.X < MarginLeft + MarginRight) size.X = MarginLeft + MarginRight;
        if (size.Y < MarginTop + MarginBottom) size.Y = MarginTop + MarginBottom;

        // Get final color (Default to White if null)
        Color finalColor = color ?? Color.White;

        // Top-Left
        // ░░══╗
        // ║   ║
        // ╚═══╝
        batch.Draw(Texture,
            new Rectangle(0, 0, MarginLeft, MarginTop).Transposed(offset),
            new Rectangle(0, 0, MarginLeft, MarginTop),
            finalColor);
        
        // Bottom-Left
        // ╔═══╗
        // ║   ║
        // ░░══╝
        batch.Draw(Texture,
            new Rectangle(0, size.Y - MarginBottom, MarginLeft, MarginBottom).Transposed(offset),
            new Rectangle(0, textureSize.Y - MarginBottom, MarginLeft, MarginBottom),
            finalColor);
        
        // Top-Right
        // ╔══░░
        // ║   ║
        // ╚═══╝
        batch.Draw(Texture,
            new Rectangle(size.X - MarginRight, 0, MarginRight, MarginTop).Transposed(offset),
            new Rectangle(textureSize.X - MarginRight, 0, MarginRight, MarginTop),
            finalColor);

        // Bottom-Right
        // ╔═══╗
        // ║   ║
        // ╚══░░
        batch.Draw(Texture,
            new Rectangle(size.X - MarginRight, size.Y - MarginBottom, MarginRight, MarginBottom).Transposed(offset),
            new Rectangle(textureSize.X - MarginRight, textureSize.Y - MarginBottom, MarginRight, MarginBottom),
            finalColor);

        // Left
        // ╔═══╗
        // ░   ║
        // ╚═══╝
        batch.Draw(Texture,
            new Rectangle(0, MarginTop, MarginLeft, sideSize.Y).Transposed(offset),
            new Rectangle(0, MarginTop, MarginLeft, textureSideSize.Y),
            finalColor);

        // Right
        // ╔═══╗
        // ║   ░
        // ╚═══╝
        batch.Draw(Texture,
            new Rectangle(size.X - MarginRight, MarginTop, MarginRight, sideSize.Y).Transposed(offset),
            new Rectangle(textureSize.X - MarginRight, MarginTop, MarginRight, textureSideSize.Y),
            finalColor);

        // Top
        // ╔░░░╗
        // ║   ║
        // ╚═══╝
        batch.Draw(Texture,
            new Rectangle(MarginLeft, 0, sideSize.X, MarginTop).Transposed(offset),
            new Rectangle(MarginLeft, 0, textureSideSize.X, MarginTop),
            finalColor);

        // Bottom
        // ╔═══╗
        // ║   ║
        // ╚░░░╝
        batch.Draw(Texture,
            new Rectangle(MarginLeft, size.Y - MarginBottom, sideSize.X, MarginBottom).Transposed(offset),
            new Rectangle(MarginLeft, textureSize.Y - MarginBottom, textureSideSize.X, MarginBottom),
            finalColor);

        // Center
        // ╔═══╗
        // ║░░░║
        // ╚═══╝
        batch.Draw(Texture,
            new Rectangle(MarginLeft, MarginTop, sideSize.X, sideSize.Y).Transposed(offset),
            new Rectangle(MarginLeft, MarginTop, textureSideSize.X, textureSideSize.Y),
            finalColor);
    }

    public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Color? color = null) => Draw(batch, position, size.ToPoint(), color);
}
