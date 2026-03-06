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
        public World()
        {
            TowerManager = new TowerManager();
        }

        public TowerManager TowerManager { get; set; }

        public void Update(GameTime gameTime)
        {
            TowerManager.Update(gameTime);
        }
    }
}
