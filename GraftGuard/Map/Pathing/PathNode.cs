using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeUtils;
using System.Collections.Generic;

namespace GraftGuard.Map.Pathing;
internal class PathNode
{
    public const float GridDistance = 32.0f;
    // Properties
    public Vector2 Position { get; private set; }
    public Circle CheckCircle { get; private set; }
    private List<PathNode> _neighbors;

    // Constructor
    /// <summary>
    /// PathNode objects are used as navigation points for enemies
    /// </summary>
    /// <param name="position">the node's position</param>
    public PathNode(Vector2 position)
    {
        Position = position;
        CheckCircle = new Circle(position.X, position.Y, 5);
    }

    /// <summary>
    /// Adds a new Neighbor to the <see cref="PathNode"/> for pathfinding
    /// </summary>
    /// <param name="neighbor"><see cref="PathNode"/> neighbor</param>
    public void AddNeighbor(PathNode neighbor)
    {
        _neighbors.Add(neighbor);
    }


    // Methods
    /// <summary>
    /// Draws the circle
    /// this should only be called if game is in debug mode
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch batch)
    {
        batch.DrawCircle(CheckCircle, Color.Red);
    }
}
