using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard
{
    internal class Player : GameObject
    {
        private static readonly float Speed = 15;
        private static Texture2D texture;

        public static void LoadContent(ContentManager content)
        {
            Player.texture = content.Load<Texture2D>("playerplaceholder");
        }

        public Player(Vector2 position) : base(position, new Vector2(50, 50), Player.texture)
        {

        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            Vector2 moveVector = inputManager.GetMovementDirection();
            base.Position += moveVector * Speed;
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            base.Draw(gameTime, base.Hitbox, batch);
        }
    }
}
