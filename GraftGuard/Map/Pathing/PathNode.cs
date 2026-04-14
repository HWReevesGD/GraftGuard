using GraftGuard.Graphics;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeUtils;
using System.Collections.Generic;

namespace GraftGuard.Map.Pathing;
internal class PathNode
{
    public const float GridDistance = 32.0f;
    public const float CheckRadius = 24.0f;
    public static bool DrawDebug = false;
    // Properties
    public Vector2 WorldPosition { get; private set; }
    public Point GridPosition { get; private set; }
    public float Cost { get; set; }
    public Circle CheckCircle => new Circle(WorldPosition, CheckRadius);
    public Color DebugColor = Color.Red;

    // Constructor
    /// <summary>
    /// PathNode objects are used as navigation points for enemies
    /// </summary>
    /// <param name="worldPosition">the node's position</param>
    public PathNode(Vector2 worldPosition, Point gridPosition)
    {
        GridPosition = gridPosition;
        WorldPosition = worldPosition;
        Cost = 1.0f;
    }

    // Methods
    /// <summary>
    /// Draws the circle
    /// this should only be called if game is in debug mode
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(DrawManager drawing)
    {
        if (!DrawDebug)
        {
            return;
        }
        drawing.DrawCircle(WorldPosition, 5.0f, DebugColor with { A = (byte)MathHelper.Clamp((int)(Cost * 20) + 50, 0, 255) });
        string text = $"{Cost}";
        drawing.DrawString(text, WorldPosition, centered: true);
    }
}
