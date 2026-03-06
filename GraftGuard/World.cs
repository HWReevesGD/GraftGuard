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
        // Fields
        private List<PathNode> pathNodes;
        private List<Enemy> enemies;

        // Properties
        public TowerManager TowerManager { get; set; }
        
        // Constructor
        public World()
        {
            pathNodes = new List<PathNode>();
            enemies = new List<Enemy>();
            TowerManager = new TowerManager();
        }

        // Methods
        public void Update(GameTime gameTime)
        {
            TowerManager.Update(gameTime);
        }

        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            TowerManager.Draw(batch, gameTime);
        }
    }
}
