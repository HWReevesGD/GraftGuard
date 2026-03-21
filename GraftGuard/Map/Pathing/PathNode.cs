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
    // Properties
    public Vector2 WorldPosition { get; private set; }
    public float Cost { get; set; }
    public Circle CheckCircle => new Circle(WorldPosition, CheckRadius);
    public Color DebugColor = Color.Red;

    // Constructor
    /// <summary>
    /// PathNode objects are used as navigation points for enemies
    /// </summary>
    /// <param name="worldPosition">the node's position</param>
    public PathNode(Vector2 worldPosition)
    {
        WorldPosition = worldPosition;
        Cost = 1.0f;
    }

    // Methods
    /// <summary>
    /// Draws the circle
    /// this should only be called if game is in debug mode
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch batch)
    {
        batch.DrawCircle(new Circle(WorldPosition, 5.0f), DebugColor with { A = (byte)MathHelper.Clamp((int)(Cost * 20), 0, 255) });
        string text = $"{Cost}";
        batch.DrawString(Fonts.Arial, text, WorldPosition - Fonts.Arial.MeasureString(text) * 0.5f, Color.White);
    }
}
