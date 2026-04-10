using GraftGuard.Grafting;
using GraftGuard.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace GraftGuard.Utility;
/// <summary>
/// Extends Classes with new Methods
/// </summary>
internal static class ClassExtensions
{
    #region Point
    public static Vector2 ToVector(this Point point)
    {
        return new Vector2(point.X, point.Y);
    }
    public static Point Divided(this Point point, int divider)
    {
        return new Point(point.X / divider, point.Y / divider);
    }

    public static Point Negative(this Point point)
    {
        return new Point(-point.X, -point.Y);
    }
    #endregion

    #region Vector
    public static Point ToPoint(this Vector2 vector)
    {
        return new Point((int)vector.X, (int)vector.Y);
    }
    public static float Angle(this Vector2 vector)
    {
        return MathF.Atan2(vector.Y, vector.X);
    }
    public static Vector2 SquareOfSmallest(this Vector2 vector)
    {
        float smallest = MathF.MinMagnitude(vector.X, vector.Y);
        return new Vector2(smallest, smallest);
    }
    public static float GetDepthSort(this Vector2 vector)
    {
        const float ySize = 100f;
        const float xSize = 0.00001f;
        return vector.Y * ySize + vector.X * xSize;
    }
    public static Point Multiply(this Point point, int scale)
    {
        return new Point(point.X * scale, point.Y * scale);
    }
    public static Point ShiftRight(this Point point, int shift)
    {
        return new Point(point.X >> shift, point.Y >> shift);
    }
    public static Point ShiftLeft(this Point point, int shift)
    {
        return new Point(point.X << shift, point.Y << shift);
    }
    public static Point BitMask(this Point point, int mask)
    {
        return new Point(point.X & mask, point.Y & mask);
    }
    public static float Average(this Vector2 vector)
    {
        return (vector.X + vector.Y) / 2.0f;
    }
    public static Vector2 Normalized(this Vector2 vector)
    {
        if (vector == Vector2.Zero) return Vector2.Zero;
        return vector / vector.Length();
    } 
    public static Vector2 MovedTowards(this Vector2 vector, Vector2 goal, float delta)
    {
        if (vector == goal)
        {
            return goal;
        }
        if (Vector2.Distance(vector, goal) <= delta)
        {
            return goal;
        }
        return vector + (goal - vector).Normalized() * delta;
    }
    public static float AngleBetween(this Vector2 thisVector, Vector2 vector)
    {
        return vector.Angle() - thisVector.Angle();
    }
    public static Vector2 Truncated(this Vector2 vector, float max)
    {
        return vector.LengthSquared() > max * max ? vector.Normalized() * max : vector;
    }

    #endregion

    #region Rectangle
    public static Rectangle Translated(this Rectangle rectangle, Point offset)
    {
        return new Rectangle(rectangle.Location + offset, rectangle.Size);
    }
    public static Rectangle LinearScaled(this Rectangle rectangle, Point scale)
    {
        return new Rectangle(rectangle.Location, rectangle.Size + scale);
    }
    public static Vector2 GetSize(this Rectangle rectangle)
    {
        return new Vector2(rectangle.Width, rectangle.Height);
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
    public static Point End(this Rectangle rectangle)
    {
        return rectangle.Location + rectangle.Size;
    }
    public static Rectangle MinkowskiDifference(this Rectangle rectangle, Rectangle other)
    {
        Point location = rectangle.Location - other.End();
        Point size = rectangle.Size + other.Size;
        return new Rectangle(
            location + size.Divided(2),
            size.Divided(2)
            );
    }
    public static Vector2 ClosestPosition(this Rectangle rectangle, Vector2 position)
    {
        return new Vector2(
            MathHelper.Clamp(position.X, rectangle.Location.X, rectangle.Location.X + rectangle.Size.X),
            MathHelper.Clamp(position.Y, rectangle.Location.Y, rectangle.Location.Y + rectangle.Size.Y)
            );
    }
    public static Point ClosestPoint(this Rectangle rectangle, Point point)
    {
        return new Point(
            MathHelper.Clamp(point.X, rectangle.Location.X, rectangle.Location.X + rectangle.Size.X),
            MathHelper.Clamp(point.Y, rectangle.Location.Y, rectangle.Location.Y + rectangle.Size.Y)
            );
    }
    public static Point ClosestBoundsPoint(this Rectangle rectangle, Point point)
    {
        Point min = rectangle.Location;
        Point max = rectangle.Location + rectangle.Size;

        float minDistance = Math.Abs(point.X - min.X);
        Point boundsPoint = new Point(min.X, point.Y);

        if (Math.Abs(max.X - point.X) < minDistance)
        {
            minDistance = Math.Abs(max.X - point.X);
            boundsPoint = new Point(max.X, point.Y);
        }

        if (Math.Abs(max.Y - point.Y) < minDistance)
        {
            minDistance = Math.Abs(max.Y - point.Y);
            boundsPoint = new Point(point.X, max.Y);
        }

        if (Math.Abs(min.Y - point.Y) < minDistance)
        {
            minDistance = Math.Abs(min.Y - point.Y);
            boundsPoint = new Point(point.X, min.Y);
        }

        return boundsPoint;
    }
    public static Vector2 ClosestBoundsPosition(this Rectangle rectangle, Vector2 position)
    {
        Vector2 min = rectangle.Location.ToVector();
        Vector2 max = (rectangle.Location + rectangle.Size).ToVector();

        float minDistance = Math.Abs(position.X - min.X);
        Vector2 boundsPoint = new Vector2(min.X, position.Y);

        if (Math.Abs(max.X - position.X) < minDistance)
        {
            minDistance = Math.Abs(max.X - position.X);
            boundsPoint = new Vector2(max.X, position.Y);
        }

        if (Math.Abs(max.Y - position.Y) < minDistance)
        {
            minDistance = Math.Abs(max.Y - position.Y);
            boundsPoint = new Vector2(position.X, max.Y);
        }

        if (Math.Abs(min.Y - position.Y) < minDistance)
        {
            minDistance = Math.Abs(min.Y - position.Y);
            boundsPoint = new Vector2(position.X, min.Y);
        }

        return boundsPoint;
    }
    public static bool RaycastFraction(this Rectangle rectangle, Vector2 position, Vector2 vector, out float fraction)
    {
        Point min = rectangle.Location;
        Point max = rectangle.End();

        float minXPlane = (min.X - position.X) / vector.X;
        float maxXPlane = (max.X - position.X) / vector.X;
        float minYPlane = (min.Y - position.Y) / vector.Y;
        float maxYPlane = (max.Y - position.Y) / vector.Y;

        float minPlane = MathF.Max(MathF.Min(minXPlane, maxXPlane), MathF.Min(minYPlane, maxYPlane));
        float maxPlane = MathF.Min(MathF.Max(minXPlane, maxXPlane), MathF.Max(minYPlane, maxYPlane));

        // If the location of the Max Place is negative,
        // the ray would be interesting only if it was pointing the opposite direction
        if (maxPlane < 0)
        {
            fraction = float.NegativeInfinity;
            return false;
        }

        // if the Min Plane's location is larger than the Max Plane's location, the ray doesn't intersect the Rectangle
        if (minPlane > maxPlane)
        {
            fraction = float.PositiveInfinity;
            return false;
        }

        if (minPlane < 0f)
        {
            fraction = maxPlane;
            return true;
        }
        fraction = minPlane;
        return true;
    }
    public static bool ContainsOrigin(this Rectangle rectangle)
    {
        Point min = rectangle.Location;
        Point max = rectangle.End();
        return
            min.X <= 0 &&
            max.X >= 0 &&
            min.Y <= 0 &&
            max.Y >= 0;
    }
    #endregion

    #region Texture
    public static Vector2 GetSize(this Texture2D texture)
    {
        return new Vector2(texture.Width, texture.Height);
    }
    public static Point GetSizePoint(this Texture2D texture)
    {
        return new Point(texture.Width, texture.Height);
    }
    #endregion

    #region SpriteBatch
    public static void DrawCentered(this SpriteBatch batch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, float scale = 1.0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
    {
        color ??= Color.White;
        origin ??= Vector2.Zero;
        batch.Draw(texture, position, sourceRectangle, color.Value, rotation, origin.Value + texture.GetSize() * 0.5f, scale, effects, layerDepth);
    }
    public static void DrawCenteredScaled(this SpriteBatch batch, Texture2D texture, Vector2 position, Vector2 scale, Rectangle ? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
    {
        color ??= Color.White;
        origin ??= Vector2.Zero;
        batch.Draw(texture, position, sourceRectangle, color.Value, rotation, origin.Value + texture.GetSize() * 0.5f, scale, effects, layerDepth);
    }
    public static void DrawCentered(this SpriteBatch batch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
    {
        color ??= Color.White;
        origin ??= Vector2.Zero;
        batch.Draw(texture, destinationRectangle, sourceRectangle, color.Value, rotation, origin.Value + texture.GetSize() * 0.5f, effects, layerDepth);
    }
    public static void DrawCircle(this SpriteBatch batch, Circle circle, Color? color = null, float layerDepth = 0.0f)
    {
        batch.DrawCentered(Placeholders.TextureCircle, new Rectangle(circle.Center.ToPoint(), new Point((int)(circle.Radius * 2.0f))), color: color, layerDepth: layerDepth);
    }
    public static void DrawSorted(this SpriteBatch batch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects effects = SpriteEffects.None, Vector2? sortingOriginOffset = null)
    {
        color ??= Color.White;
        origin ??= Vector2.Zero;
        scale ??= Vector2.One;
        sortingOriginOffset ??= Vector2.Zero;
        batch.Draw(texture, position, sourceRectangle, color.Value, rotation, origin.Value, scale.Value, effects, (origin + sortingOriginOffset).Value.GetDepthSort());
    }
    public static void DrawSortedCentered(this SpriteBatch batch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects effects = SpriteEffects.None, Vector2? sortingOriginOffset = null)
    {
        color ??= Color.White;
        origin ??= Vector2.Zero;
        scale ??= Vector2.One;
        sortingOriginOffset ??= Vector2.Zero;
        batch.Draw(texture, position, sourceRectangle, color.Value, rotation, origin.Value + texture.GetSize(), scale.Value, effects, (origin + texture.GetSize() + sortingOriginOffset).Value.GetDepthSort());
    }
    #endregion

    #region GameTime
    public static float Delta(this GameTime time)
    {
        return (float)time.ElapsedGameTime.TotalSeconds;
    }
    public static float Total(this GameTime time)
    {
        return (float)time.TotalGameTime.TotalSeconds;
    }
    #endregion

    #region Color
    public static Color Lerp(this Color color1, Color color2, float delta)
    {
        float r = MathHelper.Lerp(color1.R, color2.R, delta);
        float g = MathHelper.Lerp(color1.G, color2.G, delta);
        float b = MathHelper.Lerp(color1.B, color2.B, delta);
        float a = MathHelper.Lerp(color1.A, color2.A, delta);
        return new Color(r, g, b, a);
    }
    #endregion

    #region MISC

    public static ProjectileTarget GetTarget(this Source source)
    {
        switch (source)
        {
            case Source.Player:
                return ProjectileTarget.Enemy;
            case Source.Enemy:
                return ProjectileTarget.Player;
        }
        throw new UnreachableException();
    }

    public static float MoveTowardsAngle(this float value, float goal, float delta)
    {
        value = MathHelper.WrapAngle(value);
        goal = MathHelper.WrapAngle(goal);
        return value.MoveTowards(goal, delta);
    }

    private static float MoveTowards(this float value, float goal, float delta)
    {
        return MathF.Max(value - delta, MathF.Min(value + delta, goal));
    }

    public static Vector2 AngleToVector(this float angle)
    {
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }

    #endregion
}
