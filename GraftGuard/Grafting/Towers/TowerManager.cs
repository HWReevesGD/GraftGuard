using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard.Grafting.Towers
{
    internal class TowerManager
    {
        public List<Tower> Towers = new();

        public void Update(GameTime gameTime)
        {
            foreach (Tower tower in Towers)
            {
                tower.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            foreach (Tower tower in Towers)
            {
                tower.Draw(gameTime, batch);
            }
        }

        public void MakeTower(TowerDefinition tower, Vector2 position)
        {
            Towers.Add(tower.Factory(position));
        }
    }
}
