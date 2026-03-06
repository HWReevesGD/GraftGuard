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

        public Player(Vector2 position, Vector2 size, Texture2D texture) : base(position, size, texture)
        {

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            int xOffset = 0;
            int yOffset = 0;

            if (kb.IsKeyDown(Keys.W))
                yOffset--;
            if (kb.IsKeyDown(Keys.A))
                xOffset--;
            if (kb.IsKeyDown(Keys.S))
                yOffset++;
            if (kb.IsKeyDown(Keys.D))
                xOffset++;

            Vector2 moveVector = new Vector2(xOffset, yOffset);
            moveVector.Normalize();

            base.Position += moveVector * Speed;
        }
    }
}
