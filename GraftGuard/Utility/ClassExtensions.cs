using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.Utility;
internal static class ClassExtensions
{
    public static Point ToPoint(this Vector2 vector) => new Point((int)vector.X, (int)vector.Y);
    public static Vector2 ToVector(this Point point) => new Vector2(point.X, point.Y);
    public static void DrawCentered(this SpriteBatch batch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, float scale = 1.0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
    {
        if (color is null) color = Color.White;
        if (origin is null) origin = Vector2.Zero;
        batch.Draw(texture, position, sourceRectangle, color.Value, rotation, origin.Value + texture.GetSize() * 0.5f, scale, effects, layerDepth);
    }
    public static void DrawCentered(this SpriteBatch batch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
    {
        if (color is null) color = Color.White;
        if (origin is null) origin = Vector2.Zero;
        batch.Draw(texture, destinationRectangle, sourceRectangle, color.Value, rotation, origin.Value + texture.GetSize() * 0.5f, effects, layerDepth);
    }
    public static Vector2 GetSize(this Texture2D texture)
    {
        return new Vector2(texture.Width, texture.Height);
    }
    public static Point GetSizePoint(this Texture2D texture)
    {
        return new Point(texture.Width, texture.Height);
    }
    public static Rectangle Translated(this Rectangle rectangle, Point offset)
    {
        return new Rectangle(rectangle.Location + offset, rectangle.Size);
    }
    public static Vector2 GetSize(this Rectangle rectangle)
    {
        return new Vector2(rectangle.Width, rectangle.Height);
    }
    public static Vector2 SquareOfSmallest(this Vector2 vector)
    {
        float smallest = MathF.MinMagnitude(vector.X, vector.Y);
        return new Vector2(smallest, smallest);
    }
    public static Rectangle AddX(this Rectangle rectangle, int addition)
    {
        return rectangle with { X = rectangle.X + addition };
    }
    public static Rectangle AddY(this Rectangle rectangle, int addition)
    {
        return rectangle with { Y = rectangle.Y + addition };
    }
    public static bool Intersects(this Rectangle rectangle, Circle circle)
    {
        return circle.Intersects(rectangle);
    }
    public static Point Divided(this Point point, int divider) => new Point(point.X / divider, point.Y / divider);
}
