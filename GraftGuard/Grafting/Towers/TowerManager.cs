using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GraftGuard.Grafting.Towers;
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

    /// <summary>
    /// Gets the First (Arbitrary) <see cref="Tower"/> at the current Mouse's position 
    /// </summary>
    /// <param name="inputManager"><see cref="InputManager"/> to use for Mouse</param>
    /// <returns><see cref="Tower"/> found or <see cref="null"/></returns>
    public Tower? GetFirstTowerAtMousePosition(InputManager inputManager)
    {
        foreach (Tower tower in Towers)
        {
            if (tower.IsMouseOver(inputManager))
            {
                return tower;
            }
        }
        return null;
    }
}
