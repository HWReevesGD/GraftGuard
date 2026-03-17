using GraftGuard.UI;
using GraftGuard.Utility;
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
        private static readonly float Speed = 600;
        private static Texture2D texture;

        public static void LoadContent(ContentManager content)
        {
            Player.texture = content.Load<Texture2D>("playerplaceholder");
        }

        public Player(Vector2 position) : base(position, new Vector2(25, 50), Player.texture, collisionLayers: CollisionLayer.Player, collisionMasks: CollisionLayer.Solid | CollisionLayer.Terrain)
        {

        }

        public void Update(GameTime gameTime, InputManager inputManager, World world)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 moveVector = inputManager.GetMovementDirection();
            Move(moveVector * Speed * delta, world);
            world.Camera.Position = Position - Interface.ScreenCenter;
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            base.Draw(gameTime, new Rectangle(Position.ToPoint(), new Point(25, 50)), batch);
        }
    }
}
