using GraftGuard.Data;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal class PartFlaming : IPartBehavior
{
    private IntervalTimer _fireTimer = new IntervalTimer(0.05f);
    public static IPartBehavior Create() => new PartFlaming();

    public void Draw(Tower tower, TowerSettings settings, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state)
    {
        
    }

    public void OnDealDamage(Tower tower, TowerSettings settings, float damageModifier, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        
    }

    public void Update(Tower tower, TowerSettings settings, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        bool fire = _fireTimer.Update(time);

        if (fire)
        {
            projectileManager.Add(
                new ProjectileFire(partPosition, partRotation, ProjectileTarget.Enemy)
                );
        }
    }
}
