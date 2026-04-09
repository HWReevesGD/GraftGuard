using GraftGuard.Data;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal class PartShotgunning : IPartBehavior
{
    public IntervalTimer ShotTimer = new IntervalTimer(1.2f);
    public static IPartBehavior Create() => new PartShotgunning();
    public static Random random = new Random();
    public const int ShotAmount = 5;
    public const float SeparationAngle = 0.4f;
    public const float SeparationRandomness = 0.1f;
    public void Draw(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state)
    {
        
    }

    public void OnDealDamage(PartSettings settings, float damageModifier, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        
    }

    public void Update(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        bool shoot = ShotTimer.Update(time);

        if (!shoot)
        {
            return;
        }

        for (int i = 0; i < ShotAmount; i++)
        {
            int index = i - (ShotAmount / 2);
            float angle = transform.Rotation;
            projectileManager.Add(
                new ProjectileBullet(
                    part.GetEndpoint(transform),
                    angle + (index * SeparationAngle * random.NextSingle()),
                    settings.Source.GetTarget()));
        }
    }
}
