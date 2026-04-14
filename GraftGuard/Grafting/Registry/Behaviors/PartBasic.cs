using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal class PartBasic : IPartBehavior
{
    public static IPartBehavior Create() => new PartBasic();
    public readonly IntervalTimer DamageTimer = new IntervalTimer(0.25f);
    public const float DamageRadius = 16.0f;
    public static readonly Random random = new Random();
    public void Draw(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state)
    {

    }

    public void Update(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        bool dealDamage = DamageTimer.Update(time);

        if (dealDamage)
        {
            Circle damageCircle = new Circle(transform.Position, DamageRadius);

            float damage = (part.PartDamage.BaseDamage + part.CriticalModifier * random.NextSingle());
            Damage damageFinal = new Damage(damage, part.PartDamage.DamageOverTime, part.PartDamage.DamageOverTimeDuration, part.PartDamage.SpeedMod, part.PartDamage.SpeedModDuration);

            switch (settings.Source)
            {
                case Source.Player:
                    world.EnemyManager.DealDamageInAreas([], [damageCircle], damageFinal);
                    break;
                case Source.Enemy:
                    if (world.Player.Hitbox.Intersects(damageCircle))
                    {
                        world.Player.TakeDamage(transform.Position, Math.Max((int)damageFinal.BaseDamage, 1), 1.0f);
                    }
                    break;
            }

        }
    }
}
