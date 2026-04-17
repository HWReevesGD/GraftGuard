using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
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

    /// <summary>
    /// Sets up the <see cref="TowerManager"/> for a new Session
    /// </summary>
    public void Setup()
    {
        _towers = [];
    }

    public void Update(GameTime time, World world, InputManager inputManager, TimeState state)
    {
        foreach (Tower tower in _towers)
        {
            tower.Update(time, world, inputManager, state);
        }
    }

    public void Draw(DrawManager drawing, GameTime gameTime, World world, InputManager inputManager, TimeState state)
    {
        foreach (Tower tower in _towers)
        {
            tower.Draw(gameTime, drawing, world, inputManager, state);
        }
    }

    public void MakeTower(TowerDefinition tower, Vector2 position, List<PartDefinition>? parts = null)
    {
        Tower placedTower = tower.Factory(position);

        if (parts is not null)
        {
            foreach (PartDefinition part in parts)
            {
                if (part is null)
                {
                    continue;
                }

                placedTower.AttachPart(part);
            }
        }

        _towers.Add(placedTower);
        _world.UpdateAllPathCosts();
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

    public void DegradeAndDestroy()
    {
        Stack<Tower> toRemove = [];
        foreach (Tower tower in _towers)
        {
            if (tower.NightsUsed >= 3)
            {
                toRemove.Push(tower);
                continue;
            }
            tower.NightsUsed++;
        }
        while (toRemove.Count > 0)
        {
            Tower tower = toRemove.Pop();
            _towers.Remove(tower);
        }
    }
}
