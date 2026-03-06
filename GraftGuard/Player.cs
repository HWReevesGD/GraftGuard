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
    internal class Player : GameObject
    {
        private static readonly float Speed = 15;

        public Player(Vector2 position, Vector2 hitboxSize, Texture2D texture) : base(position, hitboxSize, texture)
        {

        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            Vector2 moveVector = inputManager.GetMovementDirection();
            base.Position += moveVector * Speed;
        }
    }
}
