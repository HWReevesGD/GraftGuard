using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraftGuard.Grafting.Towers;
using GraftGuard.UI;
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
        public Camera Camera { get; set; }

        // Constructor
        public World()
        {
            pathNodes = new List<PathNode>();
            enemies = new List<Enemy>();
            
            TowerManager = new TowerManager();
            TowerGrafter = new TowerGrafter(TowerManager);
            Terrain = new Terrain();

            player = new Player(Vector2.Zero);
            Camera = new Camera();
        }

        // Methods
        public void Update(GameTime gameTime, InputManager inputManager)
        {
            player.Update(gameTime, inputManager, this);
            inputManager.Update(Camera);
            TowerManager.Update(gameTime);
            TowerGrafter.Update(gameTime, inputManager);
            Terrain.Update(gameTime);
        }

        public void DrawStatic(SpriteBatch batch, GameTime gameTime)
        {
            TowerGrafter.Draw(batch, gameTime);
            var text = $"Mou: {Mouse.GetState().Position}\nMat: {Camera.WorldToScreen.Translation}\nTra: {Vector2.Transform(Mouse.GetState().Position.ToVector2(), Camera.WorldToScreen)}\nTru: {Mouse.GetState().Position.ToVector2() + Camera.Position}";
            batch.DrawString(Fonts.Arial, text, new Vector2(0, 128), Color.White);
        }

        public void DrawCamera(SpriteBatch batch, GameTime gameTime)
        {
            TowerManager.Draw(batch, gameTime);
            Terrain.Draw(batch, gameTime);
            player.Draw(gameTime, batch);
        }
    }
}
