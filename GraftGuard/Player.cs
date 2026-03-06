using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard
{
    internal class Player : GameObject
    {
        public Player(Vector2 position, Vector2 hitboxSize, Texture2D texture) : base(position, hitboxSize, texture)
        {
        }
    }
}
