using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraftGuard.Grafting.Towers;
using GraftGuard.UI.Grafting;
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
        private Player player;

        // Properties
        public TowerManager TowerManager { get; set; }
        public TowerGrafter TowerGrafter { get; set; }
        
        // Constructor
        public World()
        {
            pathNodes = new List<PathNode>();
            enemies = new List<Enemy>();
            
            // Create new TowerManagers and TowerGrafters
            TowerManager = new TowerManager();
            TowerGrafter = new TowerGrafter(TowerManager);

            player = new Player(Vector2.Zero);
        }

        // Methods
        public void Update(GameTime gameTime, InputManager inputManager)
        {
            TowerManager.Update(gameTime);
            TowerGrafter.Update(gameTime, inputManager);
            player.Update(gameTime, inputManager);
        }

        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            TowerManager.Draw(batch, gameTime);
            TowerGrafter.Draw(batch, gameTime);
            player.Draw(gameTime, batch);
        }
    }
}
