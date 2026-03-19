using Microsoft.Xna.Framework;

namespace GraftGuard;
internal struct Circle
{
    // Fields
    private Vector2 center;

    // Properties
    public float CenterX { get => center.X; set => center.X = value; }
    public float CenterY { get => center.Y; set => center.Y = value; }
    public Vector2 Center { get => center; }
    public float Radius { get; set; }

    // Constructor
    /// <summary>
    /// Constructs a <see cref="Circle"/> from an <paramref name="x"/> position, <paramref name="y"/> position, and <paramref name="radius"/>
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="radius">Radius</param>
    public Circle(float x, float y, float radius)
    {
        center = new Vector2(x, y);
        this.Radius = radius;
    }

    // Constructor
    /// <summary>
    /// Constructs a <see cref="Circle"/> from a <paramref name="center"/> position, and <paramref name="radius"/>
    /// </summary>
    /// <param name="center">Circle position</param>
    /// <param name="radius">Radius</param>
    public Circle(Vector2 center, float radius)
    {
        this.center = center;
        this.Radius = radius;
    }

    // Constructor
    /// <summary>
    /// Constructs a <see cref="Circle"/> with the given <paramref name="radius"/> at the position <see cref="Vector2.Zero"/>
    /// </summary>
    /// <param name="center">Circle position</param>
    /// <param name="radius">Radius</param>
    public Circle(float radius)
    {
        this.center = Vector2.Zero;
        this.Radius = radius;
    }

    public bool Intersects(Circle circle)
    {
        return Vector2.Distance(Center, circle.Center) < Radius + circle.Radius;
    }

    public bool Intersects(Rectangle rectangle)
    {
        // Closest point to the circle on the rectangle
        Vector2 closest = new Vector2(
            MathHelper.Clamp(CenterX, rectangle.Left, rectangle.Right),
            MathHelper.Clamp(CenterY, rectangle.Top, rectangle.Bottom)
            );
        // Check overlap
        return Vector2.Distance(Center, closest) < Radius;
    }

    public Circle Translated(Vector2 offset)
    {
        return new Circle(Center + offset, Radius);
    }
}
