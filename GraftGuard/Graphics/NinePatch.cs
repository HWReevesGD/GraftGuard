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
    public void Draw(DrawManager drawing, Rectangle box, Color? color = null, bool isUI = true, SortMode sortMode = SortMode.Bottom, int drawLayerOffset = 0) => Draw(drawing, box.Location.ToVector(), box.Size, color, isUI, sortMode, drawLayerOffset);
    public void Draw(DrawManager drawing, Vector2 position, Point size, Color? color = null, bool isUI = true, SortMode sortMode = SortMode.Bottom, int drawLayerOffset = 0)
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
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(0, 0, MarginLeft, MarginTop).Translated(offset),
            source: new Rectangle(0, 0, MarginLeft, MarginTop),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Bottom-Left
        // ╔═══╗
        // ║   ║
        // ░░══╝
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(0, size.Y - MarginBottom, MarginLeft, MarginBottom).Translated(offset),
            source: new Rectangle(0, textureSize.Y - MarginBottom, MarginLeft, MarginBottom),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Top-Right
        // ╔══░░
        // ║   ║
        // ╚═══╝
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(size.X - MarginRight, 0, MarginRight, MarginTop).Translated(offset),
            source: new Rectangle(textureSize.X - MarginRight, 0, MarginRight, MarginTop),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Bottom-Right
        // ╔═══╗
        // ║   ║
        // ╚══░░
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(size.X - MarginRight, size.Y - MarginBottom, MarginRight, MarginBottom).Translated(offset),
            source: new Rectangle(textureSize.X - MarginRight, textureSize.Y - MarginBottom, MarginRight, MarginBottom),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Left
        // ╔═══╗
        // ░   ║
        // ╚═══╝
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(0, MarginTop, MarginLeft, sideSize.Y).Translated(offset),
            source: new Rectangle(0, MarginTop, MarginLeft, textureSideSize.Y),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Right
        // ╔═══╗
        // ║   ░
        // ╚═══╝
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(size.X - MarginRight, MarginTop, MarginRight, sideSize.Y).Translated(offset),
            source: new Rectangle(textureSize.X - MarginRight, MarginTop, MarginRight, textureSideSize.Y),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Top
        // ╔░░░╗
        // ║   ║
        // ╚═══╝
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(MarginLeft, 0, sideSize.X, MarginTop).Translated(offset),
            source: new Rectangle(MarginLeft, 0, textureSideSize.X, MarginTop),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Bottom
        // ╔═══╗
        // ║   ║
        // ╚░░░╝
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(MarginLeft, size.Y - MarginBottom, sideSize.X, MarginBottom).Translated(offset),
            source: new Rectangle(MarginLeft, textureSize.Y - MarginBottom, textureSideSize.X, MarginBottom),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);

        // Center
        // ╔═══╗
        // ║░░░║
        // ╚═══╝
        drawing.Draw(
            texture: Texture,
            destination: new Rectangle(MarginLeft, MarginTop, sideSize.X, sideSize.Y).Translated(offset),
            source: new Rectangle(MarginLeft, MarginTop, textureSideSize.X, textureSideSize.Y),
            color: finalColor,
            isUi: isUI,
            sortMode: sortMode,
            drawLayer: 1 + drawLayerOffset);
    }

    public void Draw(DrawManager drawing, Vector2 position, Vector2 size, Color? color = null, bool isUi = true, SortMode sortMode = SortMode.Bottom) => Draw(drawing, position, size.ToPoint(), color, isUi, sortMode);
}
