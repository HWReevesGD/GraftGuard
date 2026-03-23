using GraftGuard.Grafting.Registry;
using GraftGuard.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GraftGuard.Grafting.Towers;
internal class TowerManager
{
    private World _world;
    private List<Tower> _towers = new();
    public IReadOnlyList<Tower> Towers => _towers.AsReadOnly();

    public TowerManager(World world)
    {
        _world = world;
    }

    public void Update(GameTime time, World world, InputManager inputManager, TimeState state)
    {
        foreach (Tower tower in _towers)
        {
            tower.Update(time, world, inputManager, state);
        }
    }

    public void Draw(SpriteBatch batch, GameTime gameTime, World world, InputManager inputManager, TimeState state)
    {
        foreach (Tower tower in _towers)
        {
            tower.Draw(gameTime, batch, world, inputManager, state);
        }
    }

    public void MakeTower(TowerDefinition tower, Vector2 position)
    {
        _towers.Add(tower.Factory(position));
        _world.UpdatePaths();
    }

    /// <summary>
    /// Gets the First (Arbitrary) <see cref="Tower"/> at the current Mouse's position 
    /// </summary>
    /// <param name="inputManager"><see cref="InputManager"/> to use for Mouse</param>
    /// <returns><see cref="Tower"/> found or <see cref="null"/></returns>
    public Tower? GetFirstTowerAtMousePosition(InputManager inputManager)
    {
        foreach (Tower tower in _towers)
        {
            if (tower.IsMouseOver(inputManager))
            {
                return tower;
            }
        }
        return null;
    }
}
