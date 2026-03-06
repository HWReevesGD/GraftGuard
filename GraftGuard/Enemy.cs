using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard
{
    internal class Enemy : GameObject
    {
        // Fields
        private Vector2 dirUnitVec;
        private float speed;

        public Enemy(Vector2 position, Vector2 hitboxSize, Texture2D texture, float speed) : base(position, hitboxSize, texture)
        {
            this.speed = speed;
            dirUnitVec = new Vector2();
        }

        // Methods
        /// <summary>
        /// Moves the enemy object towards the target
        /// </summary>
        /// <param name="target">the PathNode object that it is moving towards</param>
        public void Move(PathNode target)
        {
            // Get the unit vector of the direction from the enemy to the node
            Vector2 dirVec = target.Position - this.Position;
            dirUnitVec = dirVec / dirVec.Length();

            // Move the enemy
            this.Position += (dirUnitVec * speed);
        }
    }
}
