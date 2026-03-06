using GraftGuard.Utility;
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
    internal class GameObject
    {
        // Properties
        public Vector2 Position { get; set; }
        public Vector2 HitboxSize { get; set; }
        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y);
        public Texture2D Texture { get; set; }

        // Constructor
        public GameObject(Vector2 position, Vector2 hitboxSize, Texture2D texture)
        {
            Position = position;
            HitboxSize = hitboxSize;
            Texture = texture;
        }

        // Methods
        /// <summary>
        /// Update call that propagates down from Game1
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime, InputManager inputManager)
        {

        }

        /// <summary>
        /// Draw call that propagates down from Game1
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.Draw(Texture, Position, Color.White);
        }
    }
}
