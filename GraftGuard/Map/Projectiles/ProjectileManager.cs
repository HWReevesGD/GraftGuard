using GraftGuard.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Projectiles;

internal class ProjectileManager
{
    public List<Projectile> Projectiles = [];
    private Stack<Projectile> _toRemove = [];

    /// <summary>
    /// Sets up the <see cref="ProjectileManager"/> for a new Session
    /// </summary>
    public void Setup()
    {
        Projectiles = [];
        _toRemove = [];
    }

    public void ClearProjectiles()
    {
        foreach (Projectile projectile in Projectiles)
        {
            _toRemove.Push(projectile);
        }
    }

    public void Update(GameTime time, World world, InputManager inputManager)
    {
        // Update Projectiles
        foreach (Projectile projectile in Projectiles)
        {
            projectile.Update(this, time, world, inputManager);
        }

        // Remove Projectiles
        // This has to be done AFTER, as the above for loop cannot modify the list it is interating on
        Projectiles = Projectiles.Where((projectile) => !_toRemove.Contains(projectile)).ToList();
        _toRemove.Clear();
    }

    public void Draw(DrawManager drawing, GameTime time, World world, InputManager inputManager, bool isUi = false)
    {
        foreach (Projectile projectile in Projectiles)
        {
            projectile.Draw(drawing, time, world, inputManager, this, isUi: isUi);
        }
    }

    /// <summary>
    /// Setup a <see cref="Projectile"/> for removal from the <see cref="World"/>
    /// </summary>
    /// <param name="projectile"><see cref="Projectile"/> to remove</param>
    /// <returns>Returns true if the <see cref="Projectile"/> was found, and is going to be removed</returns>
    public bool Remove(Projectile projectile)
    {
        bool exists = Projectiles.Contains(projectile);
        if (exists)
        {
            _toRemove.Push(projectile);
        }
        return exists;
    }

    public void Add(Projectile projectile)
    {
        Projectiles.Add(projectile);
    }

    public void AddAll(IEnumerable<Projectile> projectiles)
    {
        Projectiles.AddRange(projectiles);
    }
}
