using GraftGuard.Data;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal class PartFlaming : IPartBehavior
{
    private IntervalTimer _fireTimer = new IntervalTimer(0.05f);
    public static IPartBehavior Create() => new PartFlaming();

    public void Draw(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state)
    {
        
    }

    public void Update(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        bool fire = _fireTimer.Update(time);

        if (fire)
        {
            projectileManager.Add(
                new ProjectileFire(part.GetEndpoint(transform), transform.Scale.Average(), transform.Rotation + MathF.PI / 2.0f, settings.Source.GetTarget(),
                speedModifier: settings.PartsAreVertical ? 0.5f : 1.0f,
                lifetimeModifier: settings.PartsAreVertical ? 0.5f : 1.0f,
                isBlueprint: !ReferenceEquals(world.ProjectileManager, projectileManager))
                );
        }
    }
}
