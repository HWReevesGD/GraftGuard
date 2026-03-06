using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard.Grafting
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
    }
}
