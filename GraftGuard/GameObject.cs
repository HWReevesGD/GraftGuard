using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraftGuard;

/// <summary>
/// An <see cref="enum"/> representing a set of flags used for determining what <see cref="GameObject"/>s can interact with each other
/// </summary>
[Flags]
enum CollisionLayer
{
    None = 0,
    Solid = 1,
    Player = 1 << 1,
    Terrain = 1 << 2,
    ScatteredPart = 1 << 3,
    Enemy = 1 << 4,
}

internal class GameObject
{
    // Properties
    public Vector2 Position { get; set; }
    public Vector2 HitboxSize { get; set; }
    public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y);
    public Vector2 Center => Position + HitboxSize * 0.5f;
    public Texture2D Texture { get; set; }
    public CollisionLayer CollisionLayers { get; set; }
    public CollisionLayer CollisionMasks { get; set; }

    // Constructor
    public GameObject(Vector2 position, Vector2 hitboxSize, Texture2D texture, CollisionLayer collisionLayers = CollisionLayer.None, CollisionLayer collisionMasks = CollisionLayer.None)
    {
        Position = position;
        HitboxSize = hitboxSize;
        Texture = texture;
        CollisionLayers = collisionLayers;
        CollisionMasks = collisionMasks;
    }

    // Methods
    /// <summary>
    /// Update call that propagates down from Game1
    /// </summary>
    /// <param name="gameTime">Current frame GameTime</param>
    /// <param name="inputManager">InputManager</param>
    public virtual void Update(GameTime gameTime, InputManager inputManager)
    {

    }

    /// <summary>
    /// Draw call that propagates down from Game1
    /// </summary>
    /// <param name="gameTime">Current frame GameTime</param>
    /// <param name="batch">SpriteBatch</param>
    public virtual void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.Draw(Texture, Position, Color.White);
    }

    /// <summary>
    /// Draw call that takes in a Rectangle for the drawing bounds
    /// </summary>
    /// <param name="gameTime">Current frame GameTime</param>
    /// <param name="bounds">bounds of the texture to draw</param>
    /// <param name="batch">SpriteBatch</param>
    public virtual void Draw(GameTime gameTime, Rectangle bounds, SpriteBatch batch)
    {
        batch.Draw(Texture, bounds, Color.White);
    }

    /// <summary>
    /// Moves the <see cref="GameObject"/> while colliding with terrain
    /// </summary>
    /// <param name="movement"></param>
    public virtual void MoveAndCollide(Vector2 movement, World world)
    {
        Position += CollisionSweep(movement, Hitbox, world.Terrain);
    }
    protected virtual Vector2 CollisionSweep(Vector2 movement, Rectangle box, Terrain terrain)
    {
        float moveDistance = movement.Length();
        if (moveDistance == 0.0f)
        {
            return Vector2.Zero;
        }
        foreach (Rectangle other in terrain.GetTileBoxes())
        {
            Rectangle difference = box.MinkowskiDifference(other);

            // If the MinkowskiDifference contains the origin, then the boxes are colliding,
            // so we just do normal "Push Out" collisions
            if (difference.ContainsOrigin())
            {
                box = MovedOut(box, difference);
                continue;
            }

            // Otherwise, we do swept collision
            bool collides = difference.RaycastFraction(Vector2.Zero, movement, out float fraction);

            // If there is no collision, skip this
            if (!collides)
            {
                continue;
            }

            float distance = movement.Length() * fraction;
            if (distance < moveDistance)
            {
                moveDistance = distance;
            }

            Debug.WriteLine(difference);
            Debug.WriteLine(fraction);
            return movement;
        }
        return movement;
    }
    
    /// <summary>
    /// Given a <paramref name="box"/>, and it's COLLIDING <paramref name="minkowskiDifference"/>,
    /// returns the given <paramref name="box"/> moved out of the original other box that was used
    /// to create the given <paramref name="minkowskiDifference"/>
    /// </summary>
    /// <param name="box">Box to use</param>
    /// <param name="minkowskiDifference">Difference to use</param>
    /// <returns>Moved out box</returns>
    protected virtual Rectangle MovedOut(Rectangle box, Rectangle minkowskiDifference)
    {
        Point penetrationVector = minkowskiDifference.ClosestBoundsPoint(Point.Zero);
        box.Location += penetrationVector;
        return box;
    }

    protected virtual Rectangle DoIntersections(Rectangle currentBox, Terrain terrain)
    {
        // Horizontals First
        List<Rectangle> intersections = terrain.GetOverlappingBoxes(currentBox);

        // Horizontal
        foreach (Rectangle obstacle in intersections)
        {
            Rectangle intersect = Rectangle.Intersect(currentBox, obstacle);

            // Only Horizontally process intersections with equal or smaller widths
            if (intersect.Height < intersect.Width)
            {
                continue;
            }

            currentBox.X += intersect.Width * MathF.Sign(currentBox.X - obstacle.X);
        }

        intersections = terrain.GetOverlappingBoxes(currentBox);

        // Vertical
        foreach (Rectangle obstacle in intersections)
        {
            Rectangle intersect = Rectangle.Intersect(currentBox, obstacle);

            // Only Vertically process intersections with smaller heights
            if (intersect.Height >= intersect.Width)
            {
                continue;
            }

            currentBox.Y += intersect.Height * MathF.Sign(currentBox.Y - obstacle.Y);
        }

        // Return the modified position
        return currentBox;
    }

    /// <summary>
    /// Filters a list of <see cref="GameObject"/> to objects which lie on any of the given <paramref name="layers"/> and overlap this object's Hitbox
    /// </summary>
    /// <param name="layers"><see cref="CollisionLayer"/>s to match</param>
    /// <param name="objects">List of <see cref="GameObject"/>s to filter</param>w
    /// <returns>Filtered <see cref="GameObject"/> List of all colliding objects</returns>
    public List<GameObject> GetOverlapping(CollisionLayer layers, List<GameObject> objects)
    {
        return objects.Where((obj) => obj.CollisionLayers.HasFlag(layers) && obj.Hitbox.Intersects(Hitbox)).ToList();
    }

    public static List<T> GetIntersects<T>(Rectangle checkArea, CollisionLayer layers, List<T> objects) where T : GameObject
    {
        return objects.Where((obj) => obj.CollisionLayers.HasFlag(layers) && obj.Hitbox.Intersects(checkArea)).ToList();
    }

    public static List<T> GetIntersects<T>(Circle checkCircle, CollisionLayer layers, List<T> objects) where T : GameObject
    {
        return objects.Where((obj) => obj.CollisionLayers.HasFlag(layers) && obj.Hitbox.Intersects(checkCircle)).ToList();
    }
}
