using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard
{
    internal class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        public Texture2D Texture { get; set; }



        public virtual void Draw(SpriteBatch batch)
        {

        }
    }
}
