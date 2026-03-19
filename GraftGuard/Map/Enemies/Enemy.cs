using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GraftGuard.Map.Enemies;
internal class Enemy : GameObject
{
    // Fields
    private Vector2 dirUnitVec;
    private float speed;

    public Enemy(Vector2 position, Vector2 hitboxSize, Texture2D texture, float speed) : base(position, hitboxSize, texture, collisionLayers: CollisionLayer.Enemy)
    {
        this.speed = speed;
        dirUnitVec = new Vector2();
    }

    // Methods
    /// <summary>
    /// Moves the enemy object by having it navigate along a list of PathNodes
    /// </summary>
    /// <param name="route">the PathNode objects that it is moving along</param>
    public void Move(List<PathNode> route)
    {
        PathNode target = route[0]; // Temp

        // Get the unit vector of the direction from the enemy to the node
        Vector2 dirVec = target.Position - Position;
        dirUnitVec = dirVec / dirVec.Length();

        // Move the enemy
        Position += dirUnitVec * speed;
    }
}
