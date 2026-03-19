using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard;

/// <summary>
/// An <see cref="enum"/> representing a set of flags used for determining what <see cref="GameObject"/>s can interact with each other
/// </summary>
[Flags]
enum CollisionLayer {
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
    /// Moves the <see cref="GameObject"/> while colliding with objects with any Collison Layers that match any of this object's Collision Masks.
    /// Currently only interacts with terrain, but will interact with other objects later
    /// </summary>
    /// <param name="movement"></param>
    public virtual void Move(Vector2 movement, World world)
    {
        Position += movement;
        Rectangle objectBox = Hitbox;

        // Collide with Terrain
        if (CollisionMasks.HasFlag(CollisionLayer.Terrain))
        {
            objectBox = DoIntersections(objectBox, world.Terrain.Boxes);
        }

        Position = objectBox.Location.ToVector();
    }

    protected virtual Rectangle DoIntersections(Rectangle currentBox, List<Rectangle> boxesToIntersect)
    {
        // Horizontals First
        List<Rectangle> intersections = boxesToIntersect.Where((obstacle) => obstacle.Intersects(currentBox)).ToList();

        // Horizontal
        foreach (Rectangle obstacle in intersections)
        {
            Rectangle intersect = Rectangle.Intersect(currentBox, obstacle);

            // Only Horizontally process intersections with equal or smaller widths
            if (intersect.Height < intersect.Width) continue;

            currentBox.X += intersect.Width * MathF.Sign(currentBox.X - obstacle.X);
        }

        intersections = boxesToIntersect.Where((obstacle) => obstacle.Intersects(currentBox)).ToList();

        // Vertical
        foreach (Rectangle obstacle in intersections)
        {
            Rectangle intersect = Rectangle.Intersect(currentBox, obstacle);

            // Only Vertically process intersections with smaller heights
            if (intersect.Height >= intersect.Width) continue;

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
