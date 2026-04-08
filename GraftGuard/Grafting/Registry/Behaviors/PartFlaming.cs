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

    public void Draw(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state)
    {
        
    }

    public void OnDealDamage(PartSettings settings, float damageModifier, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        
    }

    public void Update(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        bool fire = _fireTimer.Update(time);

        if (fire)
        {
            projectileManager.Add(
                new ProjectileFire(transform.Position + Vector2.Rotate(Vector2.UnitY * part.Size * transform.Scale, transform.Rotation), transform.Scale.Average(), transform.Rotation + MathF.PI / 2.0f, ProjectileTarget.Enemy,
                speedModifier: settings.PartsAreVertical ? 0.5f : 1.0f,
                lifetimeModifier: settings.PartsAreVertical ? 0.5f : 1.0f
                )
                );
        }
    }
}
