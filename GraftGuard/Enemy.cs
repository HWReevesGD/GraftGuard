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
        public Enemy(Vector2 position, Vector2 hitboxSize, Texture2D texture) : base(position, hitboxSize, texture)
        {
        }
    }
}
