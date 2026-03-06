using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraftGuard.Grafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard
{
    internal class World
    {
        private Player player;

        public World()
        {
            TowerManager = new TowerManager();
            player = new Player(Vector2.Zero);
        }

        public TowerManager TowerManager { get; set; }

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            TowerManager.Update(gameTime);
            player.Update(gameTime, inputManager);
        }

        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            TowerManager.Draw(batch, gameTime);
            player.Draw(gameTime, batch);
        }
    }
}
