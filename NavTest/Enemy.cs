
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace NavTest
{
    internal class Enemy
    {
        // Fields
        private Vector2 dirUnitVec;
        private float speed;
        private List<PathNode> route;

        // Properties
        public Vector2 Position { get; private set; }
        public Texture2D Texture { get; set; }
        public float SpeedPenalty { get; set; }

        // Constructor
        public Enemy(Vector2 position, float speed, Texture2D texture, List<PathNode> route)
        {
            dirUnitVec = new Vector2();
            this.speed = speed;
            this.Position = position;
            this.Texture = texture;
            SpeedPenalty = 0;
            this.route = route.ToList();
        }

        // Methods
        /// <summary>
        /// Moves the enemy object by having it navigate along a list of PathNodes
        /// </summary>
        public void Move()
        {
            // Get the closest node in the list
            PathNode closest = route[0];
            foreach (PathNode node in route)
            {
                if (Vector2.Distance(this.Position, node.Position) < Vector2.Distance(this.Position, closest.Position))
                    closest = node;
            }

            // Get the unit vector of the direction from the enemy to the node
            Vector2 dirVec = closest.Position - Position;
            dirUnitVec = dirVec / dirVec.Length();

            // Move the enemy
            Position += dirUnitVec * (speed - SpeedPenalty);

            // Check if the node has been reached
            if (Vector2.Distance(Position, closest.Position) <= (speed - SpeedPenalty))
            {
                route.Remove(closest);
            }
        }
    }
}
