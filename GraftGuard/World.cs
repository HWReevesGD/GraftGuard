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
        public Terrain Terrain { get; set; }

        // Constructor
        public World()
        {
            pathNodes = new List<PathNode>();
            enemies = new List<Enemy>();
            
            TowerManager = new TowerManager();
            TowerGrafter = new TowerGrafter(TowerManager);
            Terrain = new Terrain();

            player = new Player(Vector2.Zero);
        }

        // Methods
        public void Update(GameTime gameTime, InputManager inputManager)
        {
            TowerManager.Update(gameTime);
            TowerGrafter.Update(gameTime, inputManager);
            Terrain.Update(gameTime);
            player.Update(gameTime, inputManager, this);
        }

        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            TowerManager.Draw(batch, gameTime);
            TowerGrafter.Draw(batch, gameTime);
            Terrain.Draw(batch, gameTime);
            player.Draw(gameTime, batch);
        }
    }
}
